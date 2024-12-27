using FoodShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Contract.Persistence
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IReadOnlyList<Product>> SearchProduct(string query);
    }
}
