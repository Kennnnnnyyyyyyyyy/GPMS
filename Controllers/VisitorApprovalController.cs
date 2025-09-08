using Microsoft.AspNetCore.Mvc;
using Gate_Pass_management.Services;
using Microsoft.AspNetCore.Authorization;

namespace Gate_Pass_management.Controllers;

[Authorize]
public class VisitorApprovalController : Controller
{
    private readonly IGatePassWorkflowService _workflowService;

    public VisitorApprovalController(IGatePassWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    public async Task<IActionResult> Index()
    {
        var pendingAppointments = await _workflowService.GetPendingAppointmentsAsync();
        return View(pendingAppointments);
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id, string notes = "")
    {
        try
        {
            if (id <= 0)
            {
                TempData["Error"] = "Invalid appointment id (0) posted – client script didn't set hidden field.";
                return RedirectToAction(nameof(Index));
            }
            var userName = User.Identity?.Name ?? "Admin";
            await _workflowService.ApproveAppointmentAsync(id, userName, notes);
            TempData["Success"] = "Appointment approved successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error approving appointment: {ex.Message}";
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id, string reason = "")
    {
        try
        {
            if (id <= 0)
            {
                TempData["Error"] = "Invalid appointment id (0) posted – client script didn't set hidden field.";
                return RedirectToAction(nameof(Index));
            }
            var userName = User.Identity?.Name ?? "Admin";
            await _workflowService.RejectAppointmentAsync(id, userName, reason);
            TempData["Success"] = "Appointment rejected.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error rejecting appointment: {ex.Message}";
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> GeneratePass(int id)
    {
        try
        {
            var gatePass = await _workflowService.GeneratePassAsync(id);
            TempData["Success"] = $"Gate pass generated successfully. Pass Number: {gatePass.PassNumber}";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error generating gate pass: {ex.Message}";
        }
        
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> TodaysVisitors()
    {
        var visitors = await _workflowService.GetTodaysVisitorsAsync();
        return View(visitors);
    }
}
