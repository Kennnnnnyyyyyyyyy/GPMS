using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gate_Pass_management.Data;
using Microsoft.AspNetCore.Authorization;
using Gate_Pass_management.Domain.Scheduler;
using Gate_Pass_management.Services;
using Gate_Pass_management.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Gate_Pass_management.Controllers
{
    [Route("api/v1/meetings")]
    [ApiController]
    [Authorize]
    public class MeetingsApiController : ControllerBase
    {
    private readonly AppDbContext _context;
    private readonly ISchedulingService? _schedulingService;
    private readonly IHubContext<SchedulerHub>? _hub;

    public MeetingsApiController(AppDbContext context, ISchedulingService? schedulingService = null, IHubContext<SchedulerHub>? hub = null)
        {
            _context = context;
            _schedulingService = schedulingService;
            _hub = hub;
        }

        // GET: api/v1/meetings/calendar
    [HttpGet("calendar")]
    [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetCalendarMeetings(
            [FromQuery] DateTime fromUtc,
            [FromQuery] DateTime toUtc,
            [FromQuery] string officeId)
        {
            try
            {
                var query = _context.SchedulerMeetings
                    .Include(m => m.Organizer)
                    .AsQueryable();

                // Normalize to UTC
                fromUtc = DateTime.SpecifyKind(fromUtc, DateTimeKind.Utc);
                toUtc = DateTime.SpecifyKind(toUtc, DateTimeKind.Utc);

                // Include meetings that overlap the range
                query = query.Where(m => m.Start < toUtc && m.End > fromUtc);

                // Require and apply office filter
                if (string.IsNullOrWhiteSpace(officeId) || !Guid.TryParse(officeId, out var officeGuid))
                {
                    return BadRequest(new { error = "officeId is required and must be a valid GUID" });
                }
                query = query.Where(m => m.OfficeId == officeGuid);

                var meetings = await query
                    .Select(m => new
                    {
                        id = m.Id.ToString(),
                        subject = m.Subject,
                        startUtc = DateTime.SpecifyKind(m.Start, DateTimeKind.Utc),
                        endUtc = DateTime.SpecifyKind(m.End, DateTimeKind.Utc),
                        organizerId = m.OrganizerId.ToString(),
                        organizer = m.Organizer.FullName ?? "Unknown",
                        status = m.Status,
                        officeId = m.OfficeId.ToString(),
                        roomId = m.RoomId != null ? m.RoomId.ToString() : null
                    })
                    .ToListAsync();

                return Ok(meetings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load calendar meetings", details = ex.Message });
            }
        }

        // DELETE: api/v1/meetings/before?date=YYYY-MM-DD[&officeId=]
    [HttpDelete("before")]
    [AllowAnonymous]
        public async Task<ActionResult<object>> DeleteMeetingsBefore([FromQuery] DateTime date, [FromQuery] string? officeId = null)
        {
            try
            {
                var cutoffUtc = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

                var query = _context.SchedulerMeetings.AsQueryable();
                query = query.Where(m => m.End < cutoffUtc);

                if (!string.IsNullOrWhiteSpace(officeId) && Guid.TryParse(officeId, out var officeGuid))
                {
                    query = query.Where(m => m.OfficeId == officeGuid);
                }

                var meetingIds = await query.Select(m => m.Id).ToListAsync();
                if (meetingIds.Count == 0)
                {
                    return Ok(new { deletedMeetings = 0 });
                }

                // Remove attendees first
                var attendees = _context.MeetingAttendees.Where(a => meetingIds.Contains(a.MeetingId));
                _context.MeetingAttendees.RemoveRange(attendees);

                // Remove meetings
                var meetings = _context.SchedulerMeetings.Where(m => meetingIds.Contains(m.Id));
                _context.SchedulerMeetings.RemoveRange(meetings);

                var changes = await _context.SaveChangesAsync();
                return Ok(new { deletedMeetings = meetingIds.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to delete meetings", details = ex.Message });
            }
        }

        // DELETE: api/v1/meetings/past  (everything ending before today UTC)
    [HttpDelete("past")]
    [AllowAnonymous]
        public async Task<ActionResult<object>> DeletePastMeetings([FromQuery] string? officeId = null)
        {
            var todayUtc = DateTime.UtcNow.Date;
            return await DeleteMeetingsBefore(todayUtc, officeId);
        }

        // GET: api/v1/meetings/stats/today?officeId={id}
    [HttpGet("stats/today")]
    [AllowAnonymous]
        public async Task<ActionResult<object>> GetTodayStats([FromQuery] string? officeId = null)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);
            var query = _context.SchedulerMeetings.Where(m => m.Start < tomorrow && m.End > today);
            if (!string.IsNullOrWhiteSpace(officeId) && Guid.TryParse(officeId, out var officeGuid))
            {
                query = query.Where(m => m.OfficeId == officeGuid);
            }
            var count = await query.CountAsync();
            return Ok(new { todayMeetings = count });
        }

        // GET: api/v1/meetings/stats/week?officeId={id}
    [HttpGet("stats/week")]
    [AllowAnonymous]
        public async Task<ActionResult<object>> GetWeekStats([FromQuery] string? officeId = null)
        {
            var now = DateTime.UtcNow;
            var weekStart = now.Date.AddDays(-(int)now.DayOfWeek);
            var weekEnd = weekStart.AddDays(7);
            var query = _context.SchedulerMeetings.Where(m => m.Start < weekEnd && m.End > weekStart);
            if (!string.IsNullOrWhiteSpace(officeId) && Guid.TryParse(officeId, out var officeGuid))
            {
                query = query.Where(m => m.OfficeId == officeGuid);
            }
            var count = await query.CountAsync();
            return Ok(new { weekMeetings = count });
        }

        // GET: api/v1/meetings/availability?officeId={id}&date=YYYY-MM-DD
    [HttpGet("availability")]
    [AllowAnonymous]
        public async Task<ActionResult<object>> GetAvailability([FromQuery] string officeId, [FromQuery] DateTime date)
        {
            if (!Guid.TryParse(officeId, out var officeGuid)) return BadRequest(new { error = "Invalid office ID" });
            var office = await _context.Offices.FindAsync(officeGuid);
            if (office == null) return NotFound(new { error = "Office not found" });

            // Hours 10:00-18:00 local, rooms in this office
            var tz = TimeZoneInfo.FindSystemTimeZoneById(office.TimeZoneId ?? "Asia/Kolkata");
            DateTime LocalToUtc(DateTime local) => TimeZoneInfo.ConvertTimeToUtc(local, tz);

            var rooms = await _context.SchedulerRooms.Where(r => r.OfficeId == officeGuid && r.IsActive).ToListAsync();
            var startLocal = date.Date.AddHours(10);
            var endLocal = date.Date.AddHours(18);
            var startUtc = LocalToUtc(startLocal);
            var endUtc = LocalToUtc(endLocal);

            var meetings = await _context.SchedulerMeetings
                .Where(m => m.OfficeId == officeGuid && m.Start < endUtc && m.End > startUtc)
                .Select(m => new { m.RoomId, m.Start, m.End })
                .ToListAsync();

            var availability = rooms.Select(r => new
            {
                roomId = r.Id,
                roomName = r.Name,
                bookings = meetings.Where(m => m.RoomId == r.Id).Select(m => new { startUtc = m.Start, endUtc = m.End }).ToList()
            }).ToList();

            return Ok(new { date = date.Date, officeId, availability });
        }

        // POST: api/v1/meetings/propose
    [HttpPost("propose")]
    [AllowAnonymous]
    public ActionResult<object> ProposeTimeSlots([FromBody] ProposeTimeSlotsRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { error = "Invalid request data" });
                }

                if (!Guid.TryParse(request.OfficeId, out var officeGuid))
                    return BadRequest(new { error = "Invalid office ID" });

                var office = _context.Offices.Find(officeGuid);
                if (office == null) return NotFound(new { error = "Office not found" });

                // If a preferred room is provided, ensure it belongs to this office
                if (!string.IsNullOrWhiteSpace(request.PreferredRoom) && Guid.TryParse(request.PreferredRoom, out var preferredRoomGuid))
                {
                    var room = _context.SchedulerRooms.Find(preferredRoomGuid);
                    if (room == null || room.OfficeId != office.Id)
                    {
                        return BadRequest(new { error = "Preferred room does not belong to the selected office" });
                    }
                }

                var slots = GenerateTimeSlots(request, office);
                return Ok(new { slots });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to propose time slots", details = ex.Message });
            }
        }

        // POST: api/v1/meetings
        [HttpPost]
    public async Task<ActionResult<object>> CreateMeeting([FromBody] CreateMeetingRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { error = "Invalid request data" });
                }

                // Parse IDs
                if (!Guid.TryParse(request.OfficeId, out var officeGuid))
                {
                    return BadRequest(new { error = "Invalid office ID" });
                }

                if (!Guid.TryParse(request.OrganizerId, out var organizerGuid))
                {
                    return BadRequest(new { error = "Invalid organizer ID" });
                }

                Guid? roomGuid = null;
                if (!string.IsNullOrWhiteSpace(request.RoomId) && Guid.TryParse(request.RoomId, out var parsedRoomId))
                {
                    // Validate room belongs to the selected office
                    var room = await _context.SchedulerRooms.FirstOrDefaultAsync(r => r.Id == parsedRoomId);
                    if (room == null)
                    {
                        return BadRequest(new { error = "Invalid room ID" });
                    }
                    if (room.OfficeId != officeGuid)
                    {
                        return BadRequest(new { error = "Selected room does not belong to the chosen office" });
                    }
                    roomGuid = parsedRoomId;
                }

                // Normalize times to UTC
                var startUtc = DateTime.SpecifyKind(request.Start, DateTimeKind.Utc);
                var endUtc = DateTime.SpecifyKind(request.End, DateTimeKind.Utc);

                // Optional: conflict detection if room is specified
                if (roomGuid.HasValue)
                {
                    var conflict = await _context.SchedulerMeetings.AnyAsync(m =>
                        m.OfficeId == officeGuid && m.RoomId == roomGuid &&
                        m.Start < endUtc && m.End > startUtc);
                    if (conflict)
                    {
                        return Conflict(new { error = "Room is already booked for the selected time" });
                    }
                }

                // Create the meeting
                var meeting = new Meeting
                {
                    Id = Guid.NewGuid(),
                    Subject = request.Subject,
                    Description = request.Description,
                    Start = startUtc,
                    End = endUtc,
                    OrganizerId = organizerGuid,
                    OfficeId = officeGuid,
                    RoomId = roomGuid,
                    Status = "Confirmed",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.SchedulerMeetings.Add(meeting);
                await _context.SaveChangesAsync();

                // Broadcast via SignalR (best-effort)
                try
                {
                    if (_hub != null)
                    {
                        await _hub.Clients.Group(officeGuid.ToString()).SendAsync("MeetingCreated", new
                        {
                            id = meeting.Id,
                            subject = meeting.Subject,
                            startUtc = meeting.Start,
                            endUtc = meeting.End,
                            officeId = meeting.OfficeId,
                            roomId = meeting.RoomId
                        });
                    }
                }
                catch { /* ignore hub failures */ }

                return Ok(new
                {
                    id = meeting.Id.ToString(),
                    subject = meeting.Subject,
                    start = meeting.Start,
                    end = meeting.End,
                    status = meeting.Status.ToString(),
                    message = "Meeting created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to create meeting", details = ex.Message });
            }
        }

        private List<object> GenerateTimeSlots(ProposeTimeSlotsRequest request, Office office)
        {
            var slots = new List<object>();
            var currentDate = request.Date == default ? DateTime.Today : request.Date.Date;

            // Office timezone conversion
            var tz = TimeZoneInfo.FindSystemTimeZoneById(office.TimeZoneId ?? "Asia/Kolkata");
            DateTime LocalToUtc(DateTime local) => TimeZoneInfo.ConvertTimeToUtc(local, tz);

            // Business hours in local time
            var businessStartLocal = currentDate.Add(new TimeSpan(10, 0, 0)); // enforce 10:00
            var businessEndLocal = currentDate.Add(new TimeSpan(18, 0, 0));   // enforce 18:00

            // Duration in minutes
            int durationMinutes = ParseDuration(request.Duration);

            // Fetch existing meetings for this office/room that day (UTC window)
            var dayStartUtc = LocalToUtc(businessStartLocal);
            var dayEndUtc = LocalToUtc(businessEndLocal);

            var meetingsQuery = _context.SchedulerMeetings.Where(m => m.OfficeId == office.Id && m.Start < dayEndUtc && m.End > dayStartUtc);
            if (!string.IsNullOrWhiteSpace(request.PreferredRoom) && Guid.TryParse(request.PreferredRoom, out var preferredRoomGuid))
            {
                meetingsQuery = meetingsQuery.Where(m => m.RoomId == preferredRoomGuid);
            }
            var meetings = meetingsQuery.Select(m => new { m.Start, m.End, m.RoomId }).ToList();

            // Generate 30-minute slots in local time and mark availability based on conflicts
            for (var startLocal = businessStartLocal; startLocal.AddMinutes(durationMinutes) <= businessEndLocal; startLocal = startLocal.AddMinutes(30))
            {
                var endLocal = startLocal.AddMinutes(durationMinutes);
                var startUtc = LocalToUtc(startLocal);
                var endUtc = LocalToUtc(endLocal);

                var conflict = meetings.Any(m => m.Start < endUtc && m.End > startUtc);

                slots.Add(new
                {
                    startUtc,
                    endUtc,
                    available = !conflict,
                    room = request.PreferredRoom,
                });
            }

            return slots;
        }

        private int ParseDuration(string duration)
        {
            if (string.IsNullOrEmpty(duration))
                return 60; // Default 1 hour

            duration = duration.ToLower();
            if (int.TryParse(duration, out var minutesNumeric)) return minutesNumeric;
            if (duration.Contains("hour"))
            {
                if (duration.Contains("1")) return 60;
                if (duration.Contains("2")) return 120;
                if (duration.Contains("0.5") || duration.Contains("30")) return 30;
            }
            if (duration.Contains("30") || duration.Contains("min"))
                return 30;

            return 60; // Default fallback
        }
    }

    public class ProposeTimeSlotsRequest
    {
        public string OfficeId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string PreferredRoom { get; set; } = string.Empty;
        public string OrganizerId { get; set; } = string.Empty;
    }

    public class CreateMeetingRequest
    {
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string OfficeId { get; set; } = string.Empty;
        public string OrganizerId { get; set; } = string.Empty;
        public string OrganizerName { get; set; } = string.Empty;
        public string PreferredRoom { get; set; } = string.Empty;
        public string Attendees { get; set; } = string.Empty;
    public string? RoomId { get; set; }
    }
}
