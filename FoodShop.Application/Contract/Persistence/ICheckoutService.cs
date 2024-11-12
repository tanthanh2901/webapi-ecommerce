using FoodShop.Domain.Entities;

namespace FoodShop.Application.Contract.Persistence
{
    public interface ICheckoutService
    {
        Task<(Order order, string orderLink)> ProcessCheckoutAsync(int userId, int paymentMethodId);
    }
}
