using System.ComponentModel.DataAnnotations;

namespace Gate_Pass_management.Domain.GatePass;

/// <summary>
/// Represents a visitor in the gate pass system
/// </summary>
public class Visitor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public required string FullName { get; set; }
    
    [MaxLength(15)]
    public string? Mobile { get; set; }
    
    [MaxLength(100)]
    public string? Email { get; set; }
    
    [MaxLength(100)]
    public string? Company { get; set; }
    
    [MaxLength(20)]
    public string? IdType { get; set; }
    
    [MaxLength(50)]
    public string? IdNumber { get; set; }
    
    [MaxLength(500)]
    public string? PhotoUrl { get; set; }
    
    public bool IsBlocked { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<EntryLog> EntryLogs { get; set; } = new List<EntryLog>();
}
