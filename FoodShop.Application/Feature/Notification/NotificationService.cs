using Microsoft.AspNetCore.SignalR;

namespace FoodShop.Application.Feature.Notification
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyUser(int userId, string message)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }
    }
}
