using System.ComponentModel.DataAnnotations;

namespace Gate_Pass_management.Domain.GatePass;

/// <summary>
/// Represents entry/exit log for visitors at gates
/// </summary>
public class EntryLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid PassId { get; set; }
    public EntryType Type { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [Required]
    [MaxLength(50)]
    public required string GateId { get; set; }
    
    [MaxLength(100)]
    public string? GateOperator { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Pass Pass { get; set; } = null!;
}
