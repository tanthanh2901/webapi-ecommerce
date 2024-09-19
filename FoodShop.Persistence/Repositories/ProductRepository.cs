using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Persistence.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(FoodShopDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Product>> SearchProduct(string searchQuery)
        {
            IQueryable<Product> query = _dbContext.Set<Product>();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchQuery) ||
                    p.Description.Contains(searchQuery));
            }

            return await query.ToListAsync();
        }
    }
}
