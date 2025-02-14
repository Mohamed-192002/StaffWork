
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace StaffWork.Web.Service
{
    [AllowAnonymous]
    public class NotificationHub : Hub
    {
		public async Task ReceiveNotification(string message)
		{
			await Clients.All.SendAsync("ReceiveNotification", message);
		}

        public override async Task<Task> OnConnectedAsync()
        {
            return Clients.Caller.SendAsync("RegisterOnlineUser", Context.ConnectionId);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
