using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gate_Pass_management.Models;
using Gate_Pass_management.Domain.Entities;

namespace Gate_Pass_management.Domain.Scheduler;

/// <summary>
/// Represents a scheduled meeting with attendees and location details.
/// </summary>
public class Meeting
{
    /// <summary>
    /// Unique identifier for the meeting.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Meeting subject/title.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Optional meeting description or agenda.
    /// </summary>
    [StringLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Employee ID of the meeting organizer.
    /// </summary>
    public Guid OrganizerId { get; set; }

    /// <summary>
    /// Office where the meeting is scheduled.
    /// </summary>
    public Guid OfficeId { get; set; }

    /// <summary>
    /// Optional room where the meeting is scheduled (null for virtual/general meetings).
    /// </summary>
    public Guid? RoomId { get; set; }

    /// <summary>
    /// Meeting start time (UTC).
    /// </summary>
    public DateTime Start { get; set; }

    /// <summary>
    /// Meeting end time (UTC).
    /// </summary>
    public DateTime End { get; set; }

    /// <summary>
    /// Meeting status: Proposed/Scheduled/Cancelled.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Proposed";

    /// <summary>
    /// When this meeting was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this meeting was last updated (UTC). Used as concurrency token.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property to the meeting organizer.
    /// </summary>
    [ForeignKey(nameof(OrganizerId))]
    public virtual Employee Organizer { get; set; } = null!;

    /// <summary>
    /// Navigation property to the office.
    /// </summary>
    [ForeignKey(nameof(OfficeId))]
    public virtual Office Office { get; set; } = null!;

    /// <summary>
    /// Navigation property to the room (if assigned).
    /// </summary>
    [ForeignKey(nameof(RoomId))]
    public virtual Room? Room { get; set; }

    /// <summary>
    /// Navigation property for meeting attendees.
    /// </summary>
    public virtual ICollection<MeetingAttendee> Attendees { get; set; } = new List<MeetingAttendee>();
}
