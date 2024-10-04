using FoodShop.Domain.Entities;
using FoodShop.Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            // Save changes to the database
            await dbContext.SaveChangesAsync();

            // Return the created order (with the generated OrderId)
            return order;
        }

        public async Task<List<Order>> GetAllOrders(int userId)
        {
            //return await dbContext.Orders
            //    .Include(o => o.OrderDetail)
            //        .ThenInclude(od => od.Product)
            //    .Where(o => o.UserId == userId).ToListAsync();
            return await dbContext.Orders
               .Where(o => o.UserId == userId)
               .Include(o => o.OrderDetail) // Ensure OrderDetails are included
               .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await dbContext.Orders
                .Include(o => o.OrderDetail)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
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
