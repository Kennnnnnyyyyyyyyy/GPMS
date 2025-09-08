using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gate_Pass_management.Domain.Scheduler;

/// <summary>
/// Represents a meeting room within an office.
/// </summary>
public class Room
{
    /// <summary>
    /// Unique identifier for the room.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Foreign key to the office this room belongs to.
    /// </summary>
    public Guid OfficeId { get; set; }

    /// <summary>
    /// Room name or identifier (e.g., "Conference Room A", "Meeting Room 2").
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Maximum number of people the room can accommodate.
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// JSON string containing equipment details (projector, video conferencing, etc.).
    /// </summary>
    [StringLength(1000)]
    public string? EquipmentJson { get; set; }

    /// <summary>
    /// Whether the room is currently active and available for booking.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When this room record was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property to the office this room belongs to.
    /// </summary>
    [ForeignKey(nameof(OfficeId))]
    public virtual Office Office { get; set; } = null!;

    /// <summary>
    /// Navigation property for meetings scheduled in this room.
    /// </summary>
    public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
}
