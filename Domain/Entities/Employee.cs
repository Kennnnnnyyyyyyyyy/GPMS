using System.ComponentModel.DataAnnotations;

namespace Gate_Pass_management.Domain.Entities;

/// <summary>
/// Represents an employee who can receive visitors
/// </summary>
public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Employee's full name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Employee's email address
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Employee's department
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;
    
    /// <summary>
    /// Employee's designation/title
    /// </summary>
    [MaxLength(100)]
    public string? Designation { get; set; }
    
    /// <summary>
    /// Employee's contact number
    /// </summary>
    [MaxLength(20)]
    public string? ContactNumber { get; set; }
    
    /// <summary>
    /// Employee's internal ID
    /// </summary>
    [MaxLength(50)]
    public string? EmployeeId { get; set; }
    
    /// <summary>
    /// Whether the employee is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
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
