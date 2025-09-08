using Gate_Pass_management.Data;
using Gate_Pass_management.Models;
using Gate_Pass_management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gate_Pass_management.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _db;
    private readonly IVisitorMetricsService _metrics;

    public AdminController(AppDbContext db, IVisitorMetricsService metrics)
    {
        _db = db;
        _metrics = metrics;
    }

    public async Task<IActionResult> Dashboard()
    {
        var now = DateTime.Now;
        var today = now.Date;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        // Use new VisitorAppointments as primary source (fallback to legacy VisitorsEntries if any)
        var todayAppointments = await _db.VisitorAppointments
            .Where(a => a.ScheduledDate == today)
            .ToListAsync();

        var weeklyAppointments = await _db.VisitorAppointments
            .Where(a => a.ScheduledDate >= startOfWeek)
            .ToListAsync();

        var monthlyAppointments = await _db.VisitorAppointments
            .Where(a => a.ScheduledDate >= startOfMonth)
            .ToListAsync();

        // Active visitors = approved appointments whose latest entry is a CheckIn without a later CheckOut
        var activeVisitors = await _db.VisitorAppointments
            .Include(a => a.Entries)
            .Where(a => a.ScheduledDate == today && a.Status == AppointmentStatus.Approved)
            .ToListAsync();
        int activeInside = activeVisitors.Count(a => a.Entries.Any() && a.Entries
            .OrderByDescending(e => e.Timestamp).First().Action == EntryAction.CheckIn);

        // Top companies (this month)
        var topCompanies = monthlyAppointments
            .Where(a => !string.IsNullOrEmpty(a.Company))
            .GroupBy(a => a.Company!)
            .Select(g => new TopCompanyData { Company = g.Key, Count = g.Count() })
            .OrderByDescending(c => c.Count)
            .Take(5)
            .ToList();

        // Recent visitors (approved today ordered by time)
        var recent = todayAppointments
            .OrderByDescending(a => a.ApprovedAt)
            .Take(10)
            .Select(a => new SimpleVisitor
            {
                Id = a.Id,
                VisitorName = a.VisitorName,
                CompanyName = a.Company,
                VisitDateTime = a.ScheduledDate.Add(a.ScheduledTime.ToTimeSpan()),
                VisitEndDateTime = null
            })
            .ToList();

        // Daily chart (last 7 days) based on appointments scheduled per day
        var last7 = Enumerable.Range(0, 7).Select(i => today.AddDays(-i)).ToList();
        var chart = new List<DailyVisitorCount>();
        foreach (var day in last7)
        {
            var count = await _db.VisitorAppointments.CountAsync(a => a.ScheduledDate == day);
            chart.Add(new DailyVisitorCount { Date = day, Count = count });
        }
        chart = chart.OrderBy(c => c.Date).ToList();

        var dashboardData = new AdminDashboardViewModel
        {
            ActiveVisitors = activeInside,
            TodayVisitors = todayAppointments.Count,
            WeeklyVisitors = weeklyAppointments.Count,
            MonthlyVisitors = monthlyAppointments.Count,
            TotalSites = 1,
            TotalRooms = await _db.Rooms.CountAsync(),
            ActiveGatePasses = await _db.GatePasses.CountAsync(g => g.Status == Gate_Pass_management.Models.GatePassStatus.Active),
            PendingSms = 0,
            TopCompanies = topCompanies,
            RecentVisitors = recent,
            DailyVisitorsChart = chart
        };

        return View(dashboardData);
    }

    // Legacy chart helper removed; using appointments based chart above
}

public class AdminDashboardViewModel
{
    public int ActiveVisitors { get; set; }
    public int TodayVisitors { get; set; }
    public int WeeklyVisitors { get; set; }
    public int MonthlyVisitors { get; set; }
    public int TotalSites { get; set; }
    public int TotalRooms { get; set; }
    public int ActiveGatePasses { get; set; }
    public int PendingSms { get; set; }
    public List<TopCompanyData> TopCompanies { get; set; } = new();
    public List<SimpleVisitor> RecentVisitors { get; set; } = new();
    public List<DailyVisitorCount> DailyVisitorsChart { get; set; } = new();
}

public class TopCompanyData
{
    public string Company { get; set; } = null!;
    public int Count { get; set; }
}

public class DailyVisitorCount
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

public class SimpleVisitor
{
    public int Id { get; set; }
    public string? VisitorName { get; set; }
    public string? CompanyName { get; set; }
    public DateTime? VisitDateTime { get; set; }
    public DateTime? VisitEndDateTime { get; set; }
}
