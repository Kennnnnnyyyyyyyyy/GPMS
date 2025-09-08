using Microsoft.AspNetCore.Mvc;
using Gate_Pass_management.Services;
using Gate_Pass_management.Models;
using Gate_Pass_management.Data;
using Microsoft.EntityFrameworkCore;

namespace Gate_Pass_management.Controllers;

public class IntakeController : Controller
{
    private readonly IGatePassWorkflowService _workflowService;
    private readonly AppDbContext _context;

    public IntakeController(IGatePassWorkflowService workflowService, AppDbContext context)
    {
        _workflowService = workflowService;
        _context = context;
    }

    [HttpGet]
    public IActionResult Index(int? site)
    {
        var model = new CreateAppointmentRequest
        {
            VisitorName = "",
            Mobile = "",
            PurposeOfVisit = "",
            HostName = "",
            ScheduledDate = DateTime.Today.AddDays(1),
            ScheduledTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(1)),
            EstimatedDurationMinutes = 60
        };
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(CreateAppointmentRequest model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        try
        {
            var appointment = await _workflowService.CreateAppointmentAsync(model);
            
            ViewBag.AppointmentId = appointment.Id;
            ViewBag.VisitorName = appointment.VisitorName;
            ViewBag.ScheduledDate = appointment.ScheduledDate.ToString("dd-MM-yyyy");
            ViewBag.ScheduledTime = appointment.ScheduledTime.ToString("HH:mm");
            
            return View("Success");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error creating appointment: {ex.Message}");
            return View("Index", model);
        }
    }

    [HttpGet]
    public IActionResult Success()
    {
        return View();
    }
}
