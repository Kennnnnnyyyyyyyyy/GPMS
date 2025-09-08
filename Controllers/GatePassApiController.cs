using Microsoft.AspNetCore.Mvc;
using Gate_Pass_management.Services;
using Gate_Pass_management.Models;
using Gate_Pass_management.Hubs;

namespace Gate_Pass_management.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GatePassApiController : ControllerBase
{
    private readonly IGatePassWorkflowService _workflowService;
    private readonly IPdfGenerationService _pdfService;
    private readonly IVisitorTrackingNotifier _notifier;

    public GatePassApiController(
        IGatePassWorkflowService workflowService,
        IPdfGenerationService pdfService,
        IVisitorTrackingNotifier notifier)
    {
        _workflowService = workflowService;
        _pdfService = pdfService;
        _notifier = notifier;
    }

    [HttpPost("appointment")]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
    {
        try
        {
            var appointment = await _workflowService.CreateAppointmentAsync(request);
            await _notifier.NotifyAppointmentCreated(appointment.Id, appointment.VisitorName);
            
            return Ok(new { 
                AppointmentId = appointment.Id,
                Status = appointment.Status.ToString(),
                Message = "Appointment created successfully. Awaiting approval."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("appointment/{appointmentId}/approve")]
    public async Task<IActionResult> ApproveAppointment(int appointmentId, [FromBody] ApprovalRequest request)
    {
        try
        {
            var appointment = await _workflowService.ApproveAppointmentAsync(
                appointmentId, request.ApprovedBy, request.Notes);
            
            await _notifier.NotifyAppointmentApproved(appointmentId, appointment.VisitorName);
            
            return Ok(new { 
                AppointmentId = appointmentId,
                Status = appointment.Status.ToString(),
                Message = "Appointment approved successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("appointment/{appointmentId}/reject")]
    public async Task<IActionResult> RejectAppointment(int appointmentId, [FromBody] RejectionRequest request)
    {
        try
        {
            var appointment = await _workflowService.RejectAppointmentAsync(
                appointmentId, request.RejectedBy, request.Reason);
            
            await _notifier.NotifyAppointmentRejected(appointmentId, appointment.VisitorName);
            
            return Ok(new { 
                AppointmentId = appointmentId,
                Status = appointment.Status.ToString(),
                Message = "Appointment rejected."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("appointment/{appointmentId}/generate-pass")]
    public async Task<IActionResult> GeneratePass(int appointmentId)
    {
        try
        {
            var gatePass = await _workflowService.GeneratePassAsync(appointmentId);
            return Ok(new { 
                PassNumber = gatePass.PassNumber,
                QRCodeValue = gatePass.QRCodeValue,
                ValidFrom = gatePass.ValidFromUtc,
                ValidTo = gatePass.ValidToUtc,
                Message = "Gate pass generated successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("checkin")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
    {
        try
        {
            var entry = await _workflowService.CheckInAsync(
                request.QRCodeValue, request.GateNumber, request.SecurityPersonnel);
            
            var count = await _workflowService.GetCurrentVisitorCountAsync();
            await _notifier.NotifyVisitorCountChanged(count);
            
            return Ok(new { 
                Message = "Visitor checked in successfully.",
                Timestamp = entry.Timestamp,
                CurrentVisitorCount = count
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request)
    {
        try
        {
            var entry = await _workflowService.CheckOutAsync(
                request.QRCodeValue, request.GateNumber, request.SecurityPersonnel);
            
            var count = await _workflowService.GetCurrentVisitorCountAsync();
            await _notifier.NotifyVisitorCountChanged(count);
            
            return Ok(new { 
                Message = "Visitor checked out successfully.",
                Timestamp = entry.Timestamp,
                CurrentVisitorCount = count
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("pending-appointments")]
    public async Task<IActionResult> GetPendingAppointments()
    {
        try
        {
            var appointments = await _workflowService.GetPendingAppointmentsAsync();
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("current-visitor-count")]
    public async Task<IActionResult> GetCurrentVisitorCount()
    {
        try
        {
            var count = await _workflowService.GetCurrentVisitorCountAsync();
            return Ok(new { Count = count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("todays-visitors")]
    public async Task<IActionResult> GetTodaysVisitors()
    {
        try
        {
            var visitors = await _workflowService.GetTodaysVisitorsAsync();
            return Ok(visitors);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("generate-intake-qr")]
    public IActionResult GenerateIntakeQR()
    {
        try
        {
            var intakeUrl = _workflowService.GenerateIntakeUrl();
            var qrCodeBase64 = _workflowService.GenerateQrCodeImageBase64(intakeUrl);
            
            return Ok(new { 
                IntakeUrl = intakeUrl,
                QRCodeBase64 = qrCodeBase64
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}

public class ApprovalRequest
{
    public required string ApprovedBy { get; set; }
    public string? Notes { get; set; }
}

public class RejectionRequest
{
    public required string RejectedBy { get; set; }
    public string? Reason { get; set; }
}

public class CheckInRequest
{
    public required string QRCodeValue { get; set; }
    public string? GateNumber { get; set; }
    public string? SecurityPersonnel { get; set; }
}

public class CheckOutRequest
{
    public required string QRCodeValue { get; set; }
    public string? GateNumber { get; set; }
    public string? SecurityPersonnel { get; set; }
}
