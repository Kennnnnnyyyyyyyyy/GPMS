using System.ComponentModel.DataAnnotations;

namespace Gate_Pass_management.Domain.Entities;

/// <summary>
/// Represents a visitor in the system
/// </summary>
public class Visitor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Visitor's full name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Visitor's contact number
    /// </summary>
    [MaxLength(20)]
    public string? ContactNumber { get; set; }
    
    /// <summary>
    /// Visitor's email address
    /// </summary>
    [MaxLength(200)]
    public string? Email { get; set; }
    
    /// <summary>
    /// Visitor's company/organization
    /// </summary>
    [MaxLength(200)]
    public string? Company { get; set; }
    
    /// <summary>
    /// Type of identification (Passport, License, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string IdType { get; set; } = string.Empty;
    
    /// <summary>
    /// Identification number
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string IdNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Visitor's photo as byte array
    /// </summary>
    public byte[]? Photo { get; set; }
    
    /// <summary>
    /// Visitor's address
    /// </summary>
    [MaxLength(500)]
    public string? Address { get; set; }
    
    /// <summary>
    /// Whether the visitor is blacklisted
    /// </summary>
    public bool IsBlacklisted { get; set; } = false;
    
    /// <summary>
    /// Reason for blacklisting
    /// </summary>
    [MaxLength(500)]
    public string? BlacklistReason { get; set; }
    
    /// <summary>
    /// Record creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Navigation property to appointments
    /// </summary>
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
