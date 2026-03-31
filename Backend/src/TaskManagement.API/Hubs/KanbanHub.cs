using Microsoft.AspNetCore.SignalR;

namespace TaskManagement.API.Hubs
{
    public class KanbanHub : Hub
    {
        public async Task JoinProjectGroup(string projectId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, projectId);
        }

        public async Task LeaveProjectGroup(string projectId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectId);
        }
    }
}
