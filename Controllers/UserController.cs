using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gate_Pass_management.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Gate_Pass_management.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/v1/user
        [HttpGet]
        public async Task<ActionResult<object>> GetCurrentUser()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized(new { error = "User not authenticated" });
                }

                // Try to find corresponding employee record
                var employee = await _context.Employees
                    .Where(e => e.Email == user.Email)
                    .FirstOrDefaultAsync();

                if (employee != null)
                {
                    return Ok(new
                    {
                        id = employee.Id.ToString(),
                        name = employee.FullName,
                        email = employee.Email,
                        department = employee.Department,
                        designation = employee.Designation
                    });
                }

                // If no employee record, create a basic user profile
                return Ok(new
                {
                    id = user.Id,
                    name = user.UserName ?? user.Email,
                    email = user.Email,
                    department = "General",
                    designation = "User"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load user profile", details = ex.Message });
            }
        }

        // GET: api/v1/user/profile
        [HttpGet("profile")]
        public async Task<ActionResult<object>> GetUserProfile()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized(new { error = "User not authenticated" });
                }

                var roles = await _userManager.GetRolesAsync(user);
                
                // Try to find corresponding employee record
                var employee = await _context.Employees
                    .Where(e => e.Email == user.Email)
                    .FirstOrDefaultAsync();

                return Ok(new
                {
                    id = employee?.Id.ToString() ?? user.Id,
                    name = employee?.FullName ?? user.UserName ?? user.Email,
                    email = user.Email,
                    department = employee?.Department ?? "General",
                    designation = employee?.Designation ?? "User",
                    roles = roles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load user profile", details = ex.Message });
            }
        }
    }
}
