
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StaffWork.Core.Models;

namespace StaffWork.Web.Service
{
    [AllowAnonymous]
    public class NotificationHub : Hub
    {
        public async Task ReceiveNotification(Notification notification)
        {
            await Clients.All.SendAsync("ReceiveNotification", notification);
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
