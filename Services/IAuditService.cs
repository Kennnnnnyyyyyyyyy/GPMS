using Gate_Pass_management.Data;
using Gate_Pass_management.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace Gate_Pass_management.Services;

public interface IAuditService
{
    Task LogAsync(string action, string entityName, object? entityId = null, object? data = null, string? userId = null, CancellationToken ct = default);
}

public class AuditService : IAuditService
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpAccessor;

    public AuditService(AppDbContext db, IHttpContextAccessor httpAccessor)
    {
        _db = db;
        _httpAccessor = httpAccessor;
    }

    public async Task LogAsync(string action, string entityName, object? entityId = null, object? data = null, string? userId = null, CancellationToken ct = default)
    {
        try
        {
            var currentUserId = userId ?? _httpAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var auditLog = new AuditLog
            {
                ActorUserId = currentUserId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId?.ToString(),
                DataJson = data != null ? JsonSerializer.Serialize(data) : null,
                CreatedUtc = DateTime.UtcNow
            };

            _db.AuditLogs.Add(auditLog);
            await _db.SaveChangesAsync(ct);
        }
        catch
        {
            // Audit logging failures shouldn't break the main flow
            // In production, consider logging to a separate system
        }
    }
}
