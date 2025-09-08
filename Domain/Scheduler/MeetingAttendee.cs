using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gate_Pass_management.Domain.Entities;
using Gate_Pass_management.Models;

namespace Gate_Pass_management.Domain.Scheduler;

/// <summary>
/// Represents an attendee's participation in a meeting with their response status.
/// </summary>
public class MeetingAttendee
{
    /// <summary>
    /// Unique identifier for this attendee record.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Foreign key to the meeting.
    /// </summary>
    public Guid MeetingId { get; set; }

    /// <summary>
    /// Foreign key to the employee attending the meeting.
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Attendee's response status: Pending/Accepted/Declined/Tentative.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string ResponseStatus { get; set; } = "Pending";

    /// <summary>
    /// Navigation property to the meeting.
    /// </summary>
    [ForeignKey(nameof(MeetingId))]
    public virtual Meeting Meeting { get; set; } = null!;

    /// <summary>
    /// Navigation property to the employee.
    /// </summary>
    [ForeignKey(nameof(EmployeeId))]
    public virtual Employee Employee { get; set; } = null!;
}
