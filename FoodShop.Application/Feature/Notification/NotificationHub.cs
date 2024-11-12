using Microsoft.AspNetCore.SignalR;

namespace FoodShop.Application.Feature.Notification
{
    public sealed class NotificationHub : Hub<INotificationHub>
    {
        public override async Task OnConnectedAsync()
        {
            // Assume user's ID is passed as a query parameter or claim in production
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task RecieveNotification(int userId, string message)
        {
            await Clients.Group(userId.ToString()).RecieveNotification(message);
        }

       
    }
}
