using Gate_Pass_management.Data;
using Gate_Pass_management.Models;
using Microsoft.EntityFrameworkCore;

namespace Gate_Pass_management.Services;

public interface IVisitorMetricsService
{
    Task<VisitorMetricsDto> GetMetricsAsync(CancellationToken ct = default);
}

public record VisitorMetricsDto(int ActiveVisitors, int TodayTotalVisitors);

public class VisitorMetricsService : IVisitorMetricsService
{
    private readonly AppDbContext _db;
    public VisitorMetricsService(AppDbContext db) => _db = db;

    public async Task<VisitorMetricsDto> GetMetricsAsync(CancellationToken ct = default)
    {
        var today = DateTime.Now.Date;
        // Count appointments scheduled today
        var todayAppointments = await _db.VisitorAppointments
            .Where(a => a.ScheduledDate == today)
            .Include(a => a.Entries)
            .AsNoTracking()
            .ToListAsync(ct);

        // Active = last entry is CheckIn without a later CheckOut
        int active = todayAppointments.Count(a => a.Entries
            .OrderByDescending(e => e.Timestamp)
            .FirstOrDefault()?.Action == EntryAction.CheckIn);

        return new VisitorMetricsDto(active, todayAppointments.Count);
    }
}
