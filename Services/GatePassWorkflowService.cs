using Gate_Pass_management.Data;
using Gate_Pass_management.Models;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace Gate_Pass_management.Services;

public interface IGatePassWorkflowService
{
    Task<VisitorAppointment> CreateAppointmentAsync(CreateAppointmentRequest request);
    Task<VisitorAppointment> ApproveAppointmentAsync(int appointmentId, string approvedBy, string? notes = null);
    Task<VisitorAppointment> RejectAppointmentAsync(int appointmentId, string rejectedBy, string? reason = null);
    Task<GatePass> GeneratePassAsync(int appointmentId);
    Task<VisitorEntry> CheckInAsync(string qrCodeValue, string? gateNumber = null, string? securityPersonnel = null);
    Task<VisitorEntry> CheckOutAsync(string qrCodeValue, string? gateNumber = null, string? securityPersonnel = null);
    Task<IEnumerable<VisitorAppointment>> GetPendingAppointmentsAsync();
    Task<int> GetCurrentVisitorCountAsync();
    Task<IEnumerable<VisitorAppointment>> GetTodaysVisitorsAsync();
    Task<VisitorAppointment?> GetAppointmentWithPassAsync(int appointmentId);
    string GenerateQrCodeImageBase64(string data);
    string GenerateIntakeUrl();
}

public class GatePassWorkflowService : IGatePassWorkflowService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public GatePassWorkflowService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<VisitorAppointment> CreateAppointmentAsync(CreateAppointmentRequest request)
    {
        var appointment = new VisitorAppointment
        {
            VisitorName = request.VisitorName,
            Mobile = request.Mobile,
            Email = request.Email,
            Company = request.Company,
            PurposeOfVisit = request.PurposeOfVisit,
            ScheduledDate = request.ScheduledDate,
            ScheduledTime = request.ScheduledTime,
            EstimatedDurationMinutes = request.EstimatedDurationMinutes,
            HostName = request.HostName,
            HostDepartment = request.HostDepartment,
            HostEmployeeId = request.HostEmployeeId,
            HostContactNumber = request.HostContactNumber,
            VisitLocation = request.VisitLocation,
            ArrivingWithVehicle = request.ArrivingWithVehicle,
            VehicleType = request.VehicleType,
            VehicleNumber = request.VehicleNumber,
            FoodRequired = request.FoodRequired,
            FoodPreference = request.FoodPreference,
            ComplimentaryDrink = request.ComplimentaryDrink,
            Status = AppointmentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<VisitorAppointment>().Add(appointment);
        await _context.SaveChangesAsync();
        
        return appointment;
    }

    public async Task<VisitorAppointment> ApproveAppointmentAsync(int appointmentId, string approvedBy, string? notes = null)
    {
        var appointment = await _context.Set<VisitorAppointment>()
            .FirstOrDefaultAsync(a => a.Id == appointmentId);

    // Legacy VisitorsEntry fallback removed

        if (appointment == null)
            throw new ArgumentException($"Appointment not found (ID={appointmentId})");
            
        if (appointment.Status != AppointmentStatus.Pending)
            throw new InvalidOperationException("Appointment is not pending approval");

        appointment.Status = AppointmentStatus.Approved;
        appointment.ApprovedAt = DateTime.UtcNow;
        appointment.ApprovedBy = approvedBy;
        appointment.ApprovalNotes = notes;

        await _context.SaveChangesAsync();
        
        return appointment;
    }

    public async Task<VisitorAppointment> RejectAppointmentAsync(int appointmentId, string rejectedBy, string? reason = null)
    {
        var appointment = await _context.Set<VisitorAppointment>()
            .FirstOrDefaultAsync(a => a.Id == appointmentId);
            
        if (appointment == null)
            throw new ArgumentException("Appointment not found");
            
        if (appointment.Status != AppointmentStatus.Pending)
            throw new InvalidOperationException("Appointment is not pending approval");

        appointment.Status = AppointmentStatus.Rejected;
        appointment.ApprovedAt = DateTime.UtcNow;
        appointment.ApprovedBy = rejectedBy;
        appointment.ApprovalNotes = reason;

        await _context.SaveChangesAsync();
        
        return appointment;
    }

    public async Task<GatePass> GeneratePassAsync(int appointmentId)
    {
        var appointment = await _context.Set<VisitorAppointment>()
            .FirstOrDefaultAsync(a => a.Id == appointmentId);
            
        if (appointment == null)
            throw new ArgumentException("Appointment not found");
            
        if (appointment.Status != AppointmentStatus.Approved)
            throw new InvalidOperationException("Appointment must be approved to generate pass");

        var passNumber = GeneratePassNumber();
        var qrCodeValue = $"GP-{passNumber}-{appointmentId}";

        var gatePass = new GatePass
        {
            PassNumber = passNumber,
            VisitorName = appointment.VisitorName,
            Mobile = appointment.Mobile,
            ValidFromUtc = appointment.ScheduledDate.Date,
            ValidToUtc = appointment.ScheduledDate.Date.AddDays(1),
            Status = (GatePassStatus)4, // Active
            QRCodeValue = qrCodeValue
        };

        _context.GatePasses.Add(gatePass);
        
        appointment.GatePassId = gatePass.Id;
        await _context.SaveChangesAsync();
        
        return gatePass;
    }

    public async Task<VisitorEntry> CheckInAsync(string qrCodeValue, string? gateNumber = null, string? securityPersonnel = null)
    {
        var gatePass = await _context.GatePasses
            .FirstOrDefaultAsync(gp => gp.QRCodeValue == qrCodeValue);
            
        if (gatePass == null)
            throw new ArgumentException("Invalid QR code");

        var appointment = await _context.Set<VisitorAppointment>()
            .Include(a => a.Entries)
            .FirstOrDefaultAsync(a => a.GatePassId == gatePass.Id);
            
        if (appointment == null)
            throw new ArgumentException("Associated appointment not found");

        // Check if visitor is already inside
        var lastEntry = appointment.Entries
            .OrderByDescending(e => e.Timestamp)
            .FirstOrDefault();
            
        if (lastEntry?.Action == EntryAction.CheckIn)
            throw new InvalidOperationException("Visitor is already checked in");

        var entry = new VisitorEntry
        {
            AppointmentId = appointment.Id,
            Action = EntryAction.CheckIn,
            Timestamp = DateTime.UtcNow,
            GateNumber = gateNumber,
            SecurityPersonnel = securityPersonnel
        };

        _context.Set<VisitorEntry>().Add(entry);
        await _context.SaveChangesAsync();
        
        return entry;
    }

    public async Task<VisitorEntry> CheckOutAsync(string qrCodeValue, string? gateNumber = null, string? securityPersonnel = null)
    {
        var gatePass = await _context.GatePasses
            .FirstOrDefaultAsync(gp => gp.QRCodeValue == qrCodeValue);
            
        if (gatePass == null)
            throw new ArgumentException("Invalid QR code");

        var appointment = await _context.Set<VisitorAppointment>()
            .Include(a => a.Entries)
            .FirstOrDefaultAsync(a => a.GatePassId == gatePass.Id);
            
        if (appointment == null)
            throw new ArgumentException("Associated appointment not found");

        // Check if visitor is inside
        var lastEntry = appointment.Entries
            .OrderByDescending(e => e.Timestamp)
            .FirstOrDefault();
            
        if (lastEntry?.Action != EntryAction.CheckIn)
            throw new InvalidOperationException("Visitor is not checked in");

        var entry = new VisitorEntry
        {
            AppointmentId = appointment.Id,
            Action = EntryAction.CheckOut,
            Timestamp = DateTime.UtcNow,
            GateNumber = gateNumber,
            SecurityPersonnel = securityPersonnel
        };

        _context.Set<VisitorEntry>().Add(entry);
        await _context.SaveChangesAsync();
        
        return entry;
    }

    public async Task<IEnumerable<VisitorAppointment>> GetPendingAppointmentsAsync()
    {
        return await _context.Set<VisitorAppointment>()
            .Where(a => a.Status == AppointmentStatus.Pending)
            .OrderBy(a => a.ScheduledDate)
            .ThenBy(a => a.ScheduledTime)
            .ToListAsync();
    }

    public async Task<int> GetCurrentVisitorCountAsync()
    {
        var today = DateTime.Today;
        var appointments = await _context.Set<VisitorAppointment>()
            .Include(a => a.Entries)
            .Where(a => a.ScheduledDate == today && a.Status == AppointmentStatus.Approved)
            .ToListAsync();

        var currentlyInside = appointments
            .Where(a => a.Entries.Any())
            .Where(a => {
                var lastEntry = a.Entries.OrderByDescending(e => e.Timestamp).First();
                return lastEntry.Action == EntryAction.CheckIn;
            })
            .Count();

        return currentlyInside;
    }

    public async Task<IEnumerable<VisitorAppointment>> GetTodaysVisitorsAsync()
    {
        var today = DateTime.Today;
        // Show visitors whose appointment is scheduled for today OR whose approval happened today.
        // This ensures that if a receptionist approves an appointment that was created for a later date
        // (or mapped from legacy data without an exact scheduled date), it still appears immediately.
        var query = _context.Set<VisitorAppointment>()
            .Include(a => a.Entries)
            .Where(a => a.ScheduledDate == today || (a.ApprovedAt.HasValue && a.ApprovedAt.Value.Date == today));

        return await query
            .OrderBy(a => a.ScheduledDate)
            .ThenBy(a => a.ScheduledTime)
            .ToListAsync();
    }

    public async Task<VisitorAppointment?> GetAppointmentWithPassAsync(int appointmentId)
    {
        return await _context.Set<VisitorAppointment>()
            .Include(a => a.GatePass)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);
    }

    public string GenerateQrCodeImageBase64(string data)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(20);
        return Convert.ToBase64String(qrCodeBytes);
    }

    public string GenerateIntakeUrl()
    {
        var baseUrl = _configuration["BaseUrl"] ?? "https://localhost:5001";
        return $"{baseUrl}/intake";
    }

    private string GeneratePassNumber()
    {
        return $"GP{DateTime.Now:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
    }
}

public class CreateAppointmentRequest
{
    public required string VisitorName { get; set; }
    public required string Mobile { get; set; }
    public string? Email { get; set; }
    public string? Company { get; set; }
    public required string PurposeOfVisit { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeOnly ScheduledTime { get; set; }
    public int EstimatedDurationMinutes { get; set; } = 60;
    public required string HostName { get; set; }
    public string? HostDepartment { get; set; }
    public string? HostEmployeeId { get; set; }
    public string? HostContactNumber { get; set; }
    // New fields
    public string? VisitLocation { get; set; }
    public bool ArrivingWithVehicle { get; set; }
    public string? VehicleType { get; set; }
    public string? VehicleNumber { get; set; }
    public bool FoodRequired { get; set; }
    public string? FoodPreference { get; set; }
    public bool ComplimentaryDrink { get; set; }
}
