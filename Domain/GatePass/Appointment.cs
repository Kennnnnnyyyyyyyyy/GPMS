using System.ComponentModel.DataAnnotations;
using Gate_Pass_management.Domain.Entities;

namespace Gate_Pass_management.Domain.GatePass;

/// <summary>
/// Represents an appointment between a visitor and employee
/// </summary>
public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid VisitorId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid SiteId { get; set; }
    
    [Required]
    [MaxLength(500)]
    public required string Purpose { get; set; }
    
    public DateTime Start { get; set; }
    public DateTime? ExpectedEnd { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = AppointmentStatus.Pending;
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Visitor Visitor { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;
    public virtual ICollection<Pass> Passes { get; set; } = new List<Pass>();
    public virtual ICollection<EntryLog> EntryLogs { get; set; } = new List<EntryLog>();
}

/// <summary>
/// Appointment status constants
/// </summary>
public static class AppointmentStatus
{
    public const string Pending = "Pending";
    public const string Approved = "Approved";
    public const string Denied = "Denied";
    public const string Cancelled = "Cancelled";
}
