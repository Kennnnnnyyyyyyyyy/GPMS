using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gate_Pass_management.Data;
using Microsoft.AspNetCore.Authorization;
using Gate_Pass_management.Domain.Scheduler;
using Gate_Pass_management.Services;

namespace Gate_Pass_management.Controllers
{
    [Route("api/v1/meetings")]
    [ApiController]
    [Authorize]
    public class MeetingsApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ISchedulingService _schedulingService;

        public MeetingsApiController(AppDbContext context, ISchedulingService schedulingService = null)
        {
            _context = context;
            _schedulingService = schedulingService;
        }

        // GET: api/v1/meetings/calendar
        [HttpGet("calendar")]
        public async Task<ActionResult<IEnumerable<object>>> GetCalendarMeetings(
            [FromQuery] DateTime fromUtc,
            [FromQuery] DateTime toUtc,
            [FromQuery] string officeId = null)
        {
            try
            {
                var query = _context.SchedulerMeetings
                    .Include(m => m.Organizer)
                    .AsQueryable();

                // Filter by date range
                query = query.Where(m => m.Start >= fromUtc && m.End <= toUtc);

                // Filter by office if specified
                if (!string.IsNullOrEmpty(officeId) && Guid.TryParse(officeId, out var officeGuid))
                {
                    query = query.Where(m => m.OfficeId == officeGuid);
                }

                var meetings = await query
                    .Select(m => new
                    {
                        id = m.Id.ToString(),
                        subject = m.Subject,
                        startUtc = m.Start,
                        endUtc = m.End,
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

        // POST: api/v1/meetings/propose
        [HttpPost("propose")]
        public ActionResult<object> ProposeTimeSlots([FromBody] ProposeTimeSlotsRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { error = "Invalid request data" });
                }

                // For simplicity, just return some sample time slots
                var baseDate = DateTime.Today.AddDays(1); // Tomorrow
                if (!string.IsNullOrEmpty(request.Date.ToString()) && request.Date != DateTime.MinValue)
                {
                    baseDate = request.Date.Date;
                }

                var slots = new List<object>();

                // Generate some sample time slots from 9 AM to 5 PM
                for (int hour = 9; hour < 17; hour += 2) // Every 2 hours
                {
                    var startTime = baseDate.AddHours(hour);
                    var endTime = startTime.AddHours(1); // 1 hour duration

                    slots.Add(new
                    {
                        start = startTime,
                        end = endTime,
                        available = true,
                        room = request.PreferredRoom ?? "Conference Room"
                    });
                }

                return Ok(new { slots = slots });
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

                // Create the meeting
                var meeting = new Meeting
                {
                    Id = Guid.NewGuid(),
                    Subject = request.Subject,
                    Description = request.Description,
                    Start = request.Start,
                    End = request.End,
                    OrganizerId = organizerGuid,
                    OfficeId = officeGuid,
                    Status = "Confirmed",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.SchedulerMeetings.Add(meeting);
                await _context.SaveChangesAsync();

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
            var currentDate = request.Date.Date;
            
            // Convert TimeOnly to DateTime for the selected date
            var businessStart = currentDate.Add(new TimeSpan(office.BusinessStart.Hour, office.BusinessStart.Minute, 0));
            var businessEnd = currentDate.Add(new TimeSpan(office.BusinessEnd.Hour, office.BusinessEnd.Minute, 0));
            
            // Parse duration (e.g., "1 hour" -> 60 minutes)
            int durationMinutes = ParseDuration(request.Duration);
            
            // Generate 30-minute time slots during business hours
            for (var time = businessStart; time.AddMinutes(durationMinutes) <= businessEnd; time = time.AddMinutes(30))
            {
                var endTime = time.AddMinutes(durationMinutes);
                
                slots.Add(new
                {
                    start = time,
                    end = endTime,
                    available = true, // TODO: Check for conflicts
                    room = request.PreferredRoom ?? "Conference Room"
                });
            }

            return slots;
        }

        private int ParseDuration(string duration)
        {
            if (string.IsNullOrEmpty(duration))
                return 60; // Default 1 hour

            duration = duration.ToLower();
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
    }
}
