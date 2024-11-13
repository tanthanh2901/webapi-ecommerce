using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Feature.Notification;
using FoodShop.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace FoodShop.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        protected readonly FoodShopDbContext _dbContext;
        private readonly NotificationService notificationService;

        public NotificationRepository(FoodShopDbContext dbContext, NotificationService notificationService)
        {
            _dbContext = dbContext;
            this.notificationService = notificationService;
        }
        public async Task<Notification> AddNotificationAsync(int userId, string message)
        {
            var notification = new Notification
            {
                AppUserId = userId,
                Message = message,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();

            await notificationService.NotifyUser(userId, message);

            return notification;
        }
    }
}
