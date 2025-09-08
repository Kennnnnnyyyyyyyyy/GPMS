namespace Gate_Pass_management.Models;

public class AuditLog
{
    public int Id { get; set; }
    public string? ActorUserId { get; set; }
    public string Action { get; set; } = null!; // e.g. CREATE_VISITOR
    public string EntityName { get; set; } = null!; // VisitorsEntry, GatePass, etc.
    public string? EntityId { get; set; }
    public string? DataJson { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
