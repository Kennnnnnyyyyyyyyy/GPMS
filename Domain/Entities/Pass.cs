using System.ComponentModel.DataAnnotations;

namespace Gate_Pass_management.Domain.Entities;

/// <summary>
/// Pass status enumeration
/// </summary>
public enum PassStatus
{
    Active = 1,
    Used = 2,
    Cancelled = 3,
    Expired = 4
}

/// <summary>
/// Represents a generated pass for an approved appointment
/// </summary>
public class Pass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Foreign key to Appointment
    /// </summary>
    public Guid AppointmentId { get; set; }
    
    /// <summary>
    /// Navigation property to Appointment
    /// </summary>
    public Appointment Appointment { get; set; } = null!;
    
    /// <summary>
    /// QR code token for validation
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string QrToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Valid from date/time
    /// </summary>
    public DateTime ValidFrom { get; set; }
    
    /// <summary>
    /// Valid until date/time
    /// </summary>
    public DateTime ValidUntil { get; set; }
    
    /// <summary>
    /// Gate number where pass is valid
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string GateNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Status of the pass
    /// </summary>
    public PassStatus Status { get; set; } = PassStatus.Active;
    
    /// <summary>
    /// Record creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Navigation property to entry logs
    /// </summary>
    public virtual ICollection<EntryLog> EntryLogs { get; set; } = new List<EntryLog>();
}
