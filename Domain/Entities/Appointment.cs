using System.ComponentModel.DataAnnotations;

namespace Gate_Pass_management.Domain.Entities;

/// <summary>
/// Appointment status enumeration
/// </summary>
public enum AppointmentStatus
{
    Pending = 1,
    Approved = 2,
    Denied = 3,
    Cancelled = 4,
    Completed = 5,
    Expired = 6
}

/// <summary>
/// Represents a visitor appointment with an employee
/// </summary>
public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Foreign key to Visitor
    /// </summary>
    public Guid VisitorId { get; set; }
    
    /// <summary>
    /// Navigation property to Visitor
    /// </summary>
    public Visitor Visitor { get; set; } = null!;
    
    /// <summary>
    /// Foreign key to Employee
    /// </summary>
    public Guid EmployeeId { get; set; }
    
    /// <summary>
    /// Navigation property to Employee
    /// </summary>
    public Employee Employee { get; set; } = null!;
    
    /// <summary>
    /// Purpose of the visit
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Purpose { get; set; } = string.Empty;
    
    /// <summary>
    /// Scheduled date and time
    /// </summary>
    public DateTime ScheduledDate { get; set; }
    
    /// <summary>
    /// Duration in minutes
    /// </summary>
    public int Duration { get; set; } = 60;
    
    /// <summary>
    /// Meeting location
    /// </summary>
    [MaxLength(200)]
    public string? Location { get; set; }
    
    /// <summary>
    /// Status of the appointment
    /// </summary>
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    
    /// <summary>
    /// Approval or rejection notes
    /// </summary>
    [MaxLength(1000)]
    public string? ApprovalNotes { get; set; }
    
    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Record creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Navigation property to Pass
    /// </summary>
    public Pass? Pass { get; set; }
}
