using System.ComponentModel.DataAnnotations;

namespace Gate_Pass_management.Models;

/// <summary>
/// Enhanced visitor appointment/request model for gate pass workflow
/// </summary>
public class VisitorAppointment
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public required string VisitorName { get; set; }
    
    [Required]
    [MaxLength(15)]
    public required string Mobile { get; set; }
    
    [MaxLength(100)]
    public string? Email { get; set; }
    
    [MaxLength(100)]
    public string? Company { get; set; }
    
    [Required]
    [MaxLength(200)]
    public required string PurposeOfVisit { get; set; }
    
    [Required]
    public DateTime ScheduledDate { get; set; }
    
    [Required]
    public TimeOnly ScheduledTime { get; set; }
    
    public int EstimatedDurationMinutes { get; set; } = 60;
    
    [Required]
    [MaxLength(100)]
    public required string HostName { get; set; }
    
    [MaxLength(100)]
    public string? HostDepartment { get; set; }
    
    [MaxLength(20)]
    public string? HostEmployeeId { get; set; }
    
    [MaxLength(15)]
    public string? HostContactNumber { get; set; }
    
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    
    [MaxLength(500)]
    public string? ApprovalNotes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }

    // Visit location (e.g., Main Office, Mumbai, Chennai, Ahmedabad)
    [MaxLength(100)]
    public string? VisitLocation { get; set; }

    // Vehicle/parking details
    public bool ArrivingWithVehicle { get; set; }
    [MaxLength(20)]
    public string? VehicleType { get; set; } // "2-wheeler" | "4-wheeler"
    [MaxLength(20)]
    public string? VehicleNumber { get; set; }

    // Food and refreshments options
    public bool FoodRequired { get; set; }
    [MaxLength(20)]
    public string? FoodPreference { get; set; } // "Veg" | "Non-Veg" | "Both"
    public bool ComplimentaryDrink { get; set; }
    
    // Foreign keys
    public int? GatePassId { get; set; }
    
    // Navigation properties
    public GatePass? GatePass { get; set; }
    public ICollection<VisitorEntry> Entries { get; set; } = new List<VisitorEntry>();
}

/// <summary>
/// Tracks visitor entry/exit at gates
/// </summary>
public class VisitorEntry
{
    public int Id { get; set; }
    
    public int AppointmentId { get; set; }
    public VisitorAppointment Appointment { get; set; } = null!;
    
    public EntryAction Action { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [MaxLength(20)]
    public string? GateNumber { get; set; }
    
    [MaxLength(100)]
    public string? SecurityPersonnel { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    // For tracking visitor status
    public bool IsCurrentlyInside => 
        Action == EntryAction.CheckIn && 
        !Appointment.Entries.Any(e => e.Id > Id && e.Action == EntryAction.CheckOut);
}

public enum AppointmentStatus
{
    Pending,
    Approved,
    Rejected,
    Expired,
    Completed
}

public enum EntryAction
{
    CheckIn,
    CheckOut
}
