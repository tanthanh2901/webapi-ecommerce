using FoodShop.Domain.Entities;

namespace FoodShop.Application.Contract.Persistence
{
    public interface INotificationRepository
    {
        Task<Notification> AddNotificationAsync(int userId, string message);
    }
}
