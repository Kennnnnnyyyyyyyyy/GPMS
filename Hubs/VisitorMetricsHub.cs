using Gate_Pass_management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Gate_Pass_management.Hubs;

[Authorize]
public class VisitorMetricsHub : Hub
{
    private readonly IVisitorMetricsService _metrics;
    public VisitorMetricsHub(IVisitorMetricsService metrics) => _metrics = metrics;

    public async Task RequestMetrics()
    {
        var data = await _metrics.GetMetricsAsync();
        await Clients.Caller.SendAsync("metricsUpdated", data.ActiveVisitors, data.TodayTotalVisitors);
    }
}
