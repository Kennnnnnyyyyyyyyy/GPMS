using Microsoft.AspNetCore.SignalR;
using Gate_Pass_management.Services;

namespace Gate_Pass_management.Hubs;

public class VisitorTrackingHub : Hub
{
    private readonly IGatePassWorkflowService _workflowService;

    public VisitorTrackingHub(IGatePassWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task GetCurrentVisitorCount()
    {
        var count = await _workflowService.GetCurrentVisitorCountAsync();
        await Clients.Caller.SendAsync("VisitorCountUpdate", count);
    }

    public async Task RequestTodaysVisitors()
    {
        var visitors = await _workflowService.GetTodaysVisitorsAsync();
        await Clients.Caller.SendAsync("TodaysVisitorsUpdate", visitors);
    }
}

public interface IVisitorTrackingNotifier
{
    Task NotifyVisitorCountChanged(int newCount);
    Task NotifyVisitorCheckedIn(string visitorName, DateTime timestamp);
    Task NotifyVisitorCheckedOut(string visitorName, DateTime timestamp);
    Task NotifyAppointmentCreated(int appointmentId, string visitorName);
    Task NotifyAppointmentApproved(int appointmentId, string visitorName);
    Task NotifyAppointmentRejected(int appointmentId, string visitorName);
}

public class VisitorTrackingNotifier : IVisitorTrackingNotifier
{
    private readonly IHubContext<VisitorTrackingHub> _hubContext;

    public VisitorTrackingNotifier(IHubContext<VisitorTrackingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyVisitorCountChanged(int newCount)
    {
        await _hubContext.Clients.All.SendAsync("VisitorCountUpdate", newCount);
    }

    public async Task NotifyVisitorCheckedIn(string visitorName, DateTime timestamp)
    {
        await _hubContext.Clients.All.SendAsync("VisitorCheckedIn", new { 
            VisitorName = visitorName, 
            Timestamp = timestamp 
        });
    }

    public async Task NotifyVisitorCheckedOut(string visitorName, DateTime timestamp)
    {
        await _hubContext.Clients.All.SendAsync("VisitorCheckedOut", new { 
            VisitorName = visitorName, 
            Timestamp = timestamp 
        });
    }

    public async Task NotifyAppointmentCreated(int appointmentId, string visitorName)
    {
        await _hubContext.Clients.Group("admins").SendAsync("AppointmentCreated", new { 
            AppointmentId = appointmentId, 
            VisitorName = visitorName 
        });
    }

    public async Task NotifyAppointmentApproved(int appointmentId, string visitorName)
    {
        await _hubContext.Clients.All.SendAsync("AppointmentApproved", new { 
            AppointmentId = appointmentId, 
            VisitorName = visitorName 
        });
    }

    public async Task NotifyAppointmentRejected(int appointmentId, string visitorName)
    {
        await _hubContext.Clients.All.SendAsync("AppointmentRejected", new { 
            AppointmentId = appointmentId, 
            VisitorName = visitorName 
        });
    }
}
