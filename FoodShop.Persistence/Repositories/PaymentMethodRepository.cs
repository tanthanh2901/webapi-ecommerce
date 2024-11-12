using FoodShop.Application.Contract.Persistence;
using FoodShop.Domain.Entities;

namespace FoodShop.Persistence.Repositories
{
    public class PaymentMethodRepository : BaseRepository<PaymentMethod>, IPaymentMethodRepository
    {
        public PaymentMethodRepository(FoodShopDbContext dbContext) : base(dbContext)
        {
        }
    }
}
