using FoodShop.Application.Contract.Persistence;
using FoodShop.Domain.Entities;

namespace FoodShop.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        protected readonly FoodShopDbContext _dbContext;

        public NotificationRepository(FoodShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Notification> AddAsync(Notification entity)
        {
            _dbContext.Notifications.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
