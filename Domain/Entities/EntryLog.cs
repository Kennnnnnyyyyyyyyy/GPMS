using System.ComponentModel.DataAnnotations;

namespace Gate_Pass_management.Domain.Entities;

/// <summary>
/// Entry/Exit action types
/// </summary>
public enum EntryAction
{
    Entry = 1,
    Exit = 2
}

/// <summary>
/// Represents entry/exit logs for visitor access
/// </summary>
public class EntryLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Foreign key to Pass (nullable for new direct entry system)
    /// </summary>
    public Guid? PassId { get; set; }
    
    /// <summary>
    /// Navigation property to Pass
    /// </summary>
    public Pass? Pass { get; set; }
    
    /// <summary>
    /// Foreign key to Appointment (for direct entry tracking)
    /// </summary>
    public Guid? AppointmentId { get; set; }
    
    /// <summary>
    /// Navigation property to Appointment
    /// </summary>
    public Appointment? Appointment { get; set; }
    
    /// <summary>
    /// Foreign key to Visitor
    /// </summary>
    public Guid VisitorId { get; set; }
    
    /// <summary>
    /// Entry or Exit action (kept for backward compatibility)
    /// </summary>
    public EntryAction Action { get; set; }
    
    /// <summary>
    /// Timestamp of the action (kept for backward compatibility)
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Check-in timestamp (new field for tracking visitors inside)
    /// </summary>
    public DateTime InAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Check-out timestamp (null means visitor is still inside)
    /// </summary>
    public DateTime? OutAt { get; set; }
    
    /// <summary>
    /// Gate number where action occurred
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string GateNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Security personnel who recorded the action
    /// </summary>
    [MaxLength(100)]
    public string? SecurityPersonnel { get; set; }
    
    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Record creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Visitor Visitor { get; set; } = null!;
}
