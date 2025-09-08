using System.ComponentModel.DataAnnotations;

namespace Gate_Pass_management.Domain.Scheduler;

/// <summary>
/// Represents a physical office location with business hours and time zone configuration.
/// </summary>
public class Office
{
    /// <summary>
    /// Unique identifier for the office.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Office name (e.g., "Mumbai", "Chennai", "Ahmedabad").
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Time zone identifier for the office location (e.g., "Asia/Kolkata").
    /// </summary>
    [Required]
    [StringLength(50)]
    public string TimeZoneId { get; set; } = "Asia/Kolkata";

    /// <summary>
    /// Business hours start time (local time).
    /// </summary>
    public TimeOnly BusinessStart { get; set; } = new TimeOnly(9, 0);

    /// <summary>
    /// Business hours end time (local time).
    /// </summary>
    public TimeOnly BusinessEnd { get; set; } = new TimeOnly(18, 0);

    /// <summary>
    /// Working days pattern (MVP: "Mon-Fri").
    /// </summary>
    [StringLength(50)]
    public string WorkDays { get; set; } = "Mon-Fri";

    /// <summary>
    /// When this office record was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property for rooms in this office.
    /// </summary>
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    /// <summary>
    /// Navigation property for meetings in this office.
    /// </summary>
    public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
}
