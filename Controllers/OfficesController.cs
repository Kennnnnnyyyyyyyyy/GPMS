using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gate_Pass_management.Data;
using Gate_Pass_management.Domain.Scheduler;

namespace Gate_Pass_management.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OfficesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OfficesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/offices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetOffices()
        {
            try
            {
                var offices = await _context.Offices
                    .Where(o => !string.IsNullOrEmpty(o.Name))
                    .Select(o => new
                    {
                        id = o.Id.ToString(),
                        name = o.Name,
                        timezone = o.TimeZoneId,
                        workingHours = $"{o.BusinessStart:HH\\:mm} - {o.BusinessEnd:HH\\:mm}",
                        workingDays = o.WorkDays
                    })
                    .ToListAsync();

                return Ok(offices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load offices", details = ex.Message });
            }
        }

        // POST: api/v1/offices/seed-india
        [HttpPost("seed-india")]
        public async Task<IActionResult> SeedIndiaOffices()
        {
            try
            {
                // Check if offices already exist
                var existingOffices = await _context.Offices.CountAsync();
                if (existingOffices > 0)
                {
                    return Ok(new { message = "Offices already exist" });
                }

                // Create default Indian offices
                var offices = new List<Office>
                {
                    new Office
                    {
                        Id = Guid.NewGuid(),
                        Name = "Mumbai",
                        TimeZoneId = "Asia/Kolkata",
                        BusinessStart = new TimeOnly(9, 0), // 9:00 AM
                        BusinessEnd = new TimeOnly(18, 0),  // 6:00 PM
                        WorkDays = "Mon-Fri",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Office
                    {
                        Id = Guid.NewGuid(),
                        Name = "Chennai",
                        TimeZoneId = "Asia/Kolkata",
                        BusinessStart = new TimeOnly(9, 0), // 9:00 AM
                        BusinessEnd = new TimeOnly(18, 0),  // 6:00 PM
                        WorkDays = "Mon-Fri",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Office
                    {
                        Id = Guid.NewGuid(),
                        Name = "Ahmedabad",
                        TimeZoneId = "Asia/Kolkata",
                        BusinessStart = new TimeOnly(9, 0), // 9:00 AM
                        BusinessEnd = new TimeOnly(18, 0),  // 6:00 PM
                        WorkDays = "Mon-Fri",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                _context.Offices.AddRange(offices);
                await _context.SaveChangesAsync();

                return Ok(new { message = "India offices seeded successfully", count = offices.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to seed offices", details = ex.Message });
            }
        }

        // GET: api/v1/offices/{id}/rooms
        [HttpGet("{id}/rooms")]
        public async Task<ActionResult<IEnumerable<object>>> GetOfficeRooms(Guid id)
        {
            try
            {
                var rooms = await _context.SchedulerRooms
                    .Where(r => r.OfficeId == id)
                    .Select(r => new
                    {
                        id = r.Id.ToString(),
                        name = r.Name,
                        capacity = r.Capacity,
                        equipment = r.EquipmentJson ?? "[]"
                    })
                    .ToListAsync();

                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load rooms", details = ex.Message });
            }
        }
    }
}
