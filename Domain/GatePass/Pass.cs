using System.ComponentModel.DataAnnotations;

namespace Gate_Pass_management.Domain.GatePass;

/// <summary>
/// Represents a gate pass issued for an approved appointment
/// </summary>
public class Pass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid AppointmentId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public required string Serial { get; set; }
    
    [Required]
    [MaxLength(100)]
    public required string QrToken { get; set; }
    
    public int TemplateVersion { get; set; } = 1;
    
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    
    public bool IsRevoked { get; set; } = false;
    
    [MaxLength(500)]
    public string? RevokeReason { get; set; }
    
    public DateTime PrintedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Appointment Appointment { get; set; } = null!;
}
