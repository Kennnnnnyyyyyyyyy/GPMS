using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Gate_Pass_management.Hubs
{
	public class SchedulerHub : Hub
	{
		public async Task JoinOfficeGroup(string officeId)
		{
			if (!string.IsNullOrWhiteSpace(officeId))
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, officeId);
			}
		}

		public async Task LeaveOfficeGroup(string officeId)
		{
			if (!string.IsNullOrWhiteSpace(officeId))
			{
				await Groups.RemoveFromGroupAsync(Context.ConnectionId, officeId);
			}
		}
	}
}
