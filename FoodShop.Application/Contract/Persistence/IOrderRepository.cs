using FoodShop.Application.Dto;
using FoodShop.Domain.Entities;
using FoodShop.Domain.Enum;

namespace FoodShop.Persistence.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<List<OrderDto>> GetAllOrders(int userId);
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<List<OrderDto>> GetOrderByStatus(int userId, OrderStatus status);
    }
}
