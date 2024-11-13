using Microsoft.AspNetCore.SignalR;

namespace FoodShop.Application.Feature.Notification
{
    public sealed class NotificationHub : Hub   
    {
        public async Task SendNotification(int userId, string message)
        {
            await Clients.User(userId.ToString()).SendAsync("RecieveNotification", message);
        }      
    }
}
