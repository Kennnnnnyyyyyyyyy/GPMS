using Gate_Pass_management.Data;
using Gate_Pass_management.Models;
using Microsoft.EntityFrameworkCore;

namespace Gate_Pass_management.Services;

public interface ISchedulingService
{
    Task<bool> HasConflictAsync(int roomId, DateTime startUtc, DateTime endUtc, int? ignoreMeetingId = null, CancellationToken ct = default);
    Task<List<Meeting>> GetMeetingsForRoomAsync(int roomId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);
}

public class SchedulingService : ISchedulingService
{
    private readonly AppDbContext _db;
    public SchedulingService(AppDbContext db) => _db = db;

    public async Task<bool> HasConflictAsync(int roomId, DateTime startUtc, DateTime endUtc, int? ignoreMeetingId = null, CancellationToken ct = default)
    {
        return await _db.Meetings.AnyAsync(m => m.RoomId == roomId && (ignoreMeetingId == null || m.Id != ignoreMeetingId) &&
            m.StartUtc < endUtc && startUtc < m.EndUtc, ct);
    }

    public Task<List<Meeting>> GetMeetingsForRoomAsync(int roomId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
    {
        return _db.Meetings
            .Where(m => m.RoomId == roomId && m.StartUtc < toUtc && fromUtc < m.EndUtc)
            .OrderBy(m => m.StartUtc)
            .ToListAsync(ct);
    }
}
