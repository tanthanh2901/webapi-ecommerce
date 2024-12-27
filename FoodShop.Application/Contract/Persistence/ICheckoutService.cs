using FoodShop.Application.Dto;
using FoodShop.Application.Feature.Payment.Commands;

namespace FoodShop.Application.Contract.Persistence
{
    public interface ICheckoutService
    {
        Task<OrderDto> ProcessCheckoutAsync(PlaceOrderRequest placeOrderRequest);
        Task UpdateDbWhenPaymentSuccess(OrderDto orderDto);
    }
}
