using FoodShop.Application.Feature.Payment.VnPay;
using FoodShop.Application.Services.Payment.ZaloPay;

namespace FoodShop.Application.Services.Payment
{
    public interface IPaymentService : IVnPayService, IZaloPayService
    {       
       
    }
}
