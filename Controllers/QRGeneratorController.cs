using Microsoft.AspNetCore.Mvc;
using Gate_Pass_management.Services;
using Gate_Pass_management.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Gate_Pass_management.Controllers;

[Authorize]
public class QRGeneratorController : Controller
{
    private readonly IGatePassWorkflowService _workflowService;
    private readonly AppDbContext _context;

    public QRGeneratorController(IGatePassWorkflowService workflowService, AppDbContext context)
    {
        _workflowService = workflowService;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult GenerateIntakeQR()
    {
        try
        {
            var intakeUrl = _workflowService.GenerateIntakeUrl();
            var qrCodeBase64 = _workflowService.GenerateQrCodeImageBase64(intakeUrl);
            
            return Json(new { 
                success = true,
                intakeUrl = intakeUrl,
                qrCodeBase64 = qrCodeBase64
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
