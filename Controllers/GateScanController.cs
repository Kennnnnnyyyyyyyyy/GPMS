using Microsoft.AspNetCore.Mvc;
using Gate_Pass_management.Services;
using Gate_Pass_management.Hubs;

namespace Gate_Pass_management.Controllers;

public class GateScanController : Controller
{
    private readonly IGatePassWorkflowService _workflowService;
    private readonly IVisitorTrackingNotifier _notifier;

    public GateScanController(IGatePassWorkflowService workflowService, IVisitorTrackingNotifier notifier)
    {
        _workflowService = workflowService;
        _notifier = notifier;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProcessScan(string qrCode, string action, string gateNumber = "", string securityPersonnel = "")
    {
        try
        {
            if (action == "checkin")
            {
                var entry = await _workflowService.CheckInAsync(qrCode, gateNumber, securityPersonnel);
                var count = await _workflowService.GetCurrentVisitorCountAsync();
                await _notifier.NotifyVisitorCountChanged(count);
                
                return Json(new { 
                    success = true, 
                    message = "Visitor checked in successfully",
                    timestamp = entry.Timestamp.ToString("HH:mm:ss"),
                    currentCount = count
                });
            }
            else if (action == "checkout")
            {
                var entry = await _workflowService.CheckOutAsync(qrCode, gateNumber, securityPersonnel);
                var count = await _workflowService.GetCurrentVisitorCountAsync();
                await _notifier.NotifyVisitorCountChanged(count);
                
                return Json(new { 
                    success = true, 
                    message = "Visitor checked out successfully",
                    timestamp = entry.Timestamp.ToString("HH:mm:ss"),
                    currentCount = count
                });
            }
            else
            {
                return Json(new { success = false, message = "Invalid action specified" });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
