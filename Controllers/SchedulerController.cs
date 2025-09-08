using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gate_Pass_management.Controllers;

/// <summary>
/// Controller for the Meeting Scheduler UI interface.
/// Provides views for the advanced 3-office meeting scheduler system.
/// </summary>
[Authorize(Roles = "Admin,Reception,Employee")]
public class SchedulerController : Controller
{
    /// <summary>
    /// Main scheduler interface with office selection and calendar view.
    /// </summary>
    /// <returns>Scheduler index view</returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Calendar view for a specific office.
    /// </summary>
    /// <param name="officeId">Office identifier</param>
    /// <returns>Office calendar view</returns>
    public IActionResult Calendar(string officeId = null)
    {
        ViewBag.OfficeId = officeId;
        return View();
    }

    /// <summary>
    /// Meeting creation form.
    /// </summary>
    /// <returns>Create meeting view</returns>
    public IActionResult CreateMeeting()
    {
        return View();
    }
}
