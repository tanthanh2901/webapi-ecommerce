using FoodShop.Domain.Entities;
using FoodShop.Domain.Enum;

namespace FoodShop.Persistence.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<List<Order>> GetAllOrders(int userId);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<List<Order>> GetOrderByStatus(int userId, OrderStatus status);
    }
}
