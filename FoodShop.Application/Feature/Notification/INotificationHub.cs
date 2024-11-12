namespace FoodShop.Application.Feature.Notification
{
    public interface INotificationHub
    {
        Task RecieveNotification(string message);
    }
}
