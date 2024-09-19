using FoodShop.Domain.Entities;
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

        public Task<Order> GetOrderByIdAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            throw new NotImplementedException();
        }
    }
}
