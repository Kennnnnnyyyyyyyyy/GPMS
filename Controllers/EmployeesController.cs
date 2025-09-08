using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gate_Pass_management.Data;
using Microsoft.AspNetCore.Authorization;

namespace Gate_Pass_management.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetEmployees()
        {
            try
            {
                var employees = await _context.Employees
                    .Select(e => new
                    {
                        id = e.Id.ToString(),
                        fullName = e.FullName,
                        email = e.Email,
                        department = e.Department,
                        designation = e.Designation
                    })
                    .ToListAsync();

                if (employees.Count == 0)
                {
                    // Seed default employees if none exist
                    await SeedDefaultEmployees();
                    
                    employees = await _context.Employees
                        .Select(e => new
                        {
                            id = e.Id.ToString(),
                            fullName = e.FullName,
                            email = e.Email,
                            department = e.Department,
                            designation = e.Designation
                        })
                        .ToListAsync();
                }

                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load employees", details = ex.Message });
            }
        }

        // GET: api/v1/employees/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetEmployee(Guid id)
        {
            try
            {
                var employee = await _context.Employees
                    .Where(e => e.Id == id)
                    .Select(e => new
                    {
                        id = e.Id.ToString(),
                        fullName = e.FullName,
                        email = e.Email,
                        department = e.Department,
                        designation = e.Designation
                    })
                    .FirstOrDefaultAsync();

                if (employee == null)
                {
                    return NotFound(new { error = "Employee not found" });
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load employee", details = ex.Message });
            }
        }

        private async Task SeedDefaultEmployees()
        {
            var defaultEmployees = new[]
            {
                new Domain.Entities.Employee
                {
                    Id = Guid.NewGuid(),
                    FullName = "Admin User",
                    Email = "admin@company.com",
                    Department = "Administration",
                    Designation = "System Administrator"
                },
                new Domain.Entities.Employee
                {
                    Id = Guid.NewGuid(),
                    FullName = "Meeting Organizer",
                    Email = "organizer@company.com",
                    Department = "Operations",
                    Designation = "Meeting Coordinator"
                }
            };

            _context.Employees.AddRange(defaultEmployees);
            await _context.SaveChangesAsync();
        }
    }
}
