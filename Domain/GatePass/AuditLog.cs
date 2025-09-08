using System.ComponentModel.DataAnnotations;

namespace Gate_Pass_management.Domain.GatePass;

/// <summary>
/// Represents audit log for tracking all gate pass system activities
/// </summary>
public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public required string Actor { get; set; }
    
    [Required]
    [MaxLength(100)]
    public required string Action { get; set; }
    
    [Required]
    [MaxLength(100)]
    public required string Entity { get; set; }
    
    public Guid EntityId { get; set; }
    
    public DateTime Ts { get; set; } = DateTime.UtcNow;
    
    [MaxLength(4000)]
    public string? PayloadJson { get; set; }
}
