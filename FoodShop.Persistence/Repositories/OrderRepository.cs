using FoodShop.Application.Dto;
using FoodShop.Domain.Entities;
using FoodShop.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Persistence.Repositories
{
    internal class OrderRepository : IOrderRepository
    {
        private readonly FoodShopDbContext dbContext;

        public OrderRepository(FoodShopDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Order> CreateOrderAsync(Order order)
        {
            await dbContext.Orders.AddAsync(order);

            await dbContext.SaveChangesAsync();

            return order;
        }

        public async Task<List<OrderDto>> GetAllOrders(int userId)
        {
            //return await dbContext.Orders
            //    .Include(o => o.OrderDetail)
            //        .ThenInclude(od => od.Product)
            //    .Where(o => o.UserId == userId).ToListAsync();
            //var orders = await dbContext.Orders
            //   .Where(o => o.UserId == userId)
            //   .Include(o => o.OrderDetail)
            //   .ToListAsync();

            var orders = await dbContext.Orders
                .Where(o => o.UserId == userId)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    Items = o.OrderDetail.Select(od => new OrderDetailDto
                    {
                        OrderId = od.OrderId,
                        ProductId = od.ProductId,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice
                    }).ToList()
                }).ToListAsync();

            return orders;
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            var order = await dbContext.Orders.Where(o => o.OrderId == orderId)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    Items = o.OrderDetail.Select(od => new OrderDetailDto
                    {
                        OrderId = od.OrderId,
                        ProductId = od.ProductId,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice
                    }).ToList()
                }).FirstOrDefaultAsync();
            return order;
        }


        public async Task<List<OrderDto>> GetOrderByStatus(int userId, OrderStatus status)
        {
            var orders = await GetAllOrders(userId);
            var filteredOrders = orders.Where(o => o.Status == status).ToList();

            return filteredOrders;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await dbContext.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                return false;
            }

            order.Status = status;
            dbContext.Orders.Update(order);
            await dbContext.SaveChangesAsync();
            return true;

        }
    }
}
