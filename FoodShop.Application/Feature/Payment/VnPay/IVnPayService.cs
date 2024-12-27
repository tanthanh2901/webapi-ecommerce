using FoodShop.Application.Dto;
using Microsoft.AspNetCore.Http;

namespace FoodShop.Application.Feature.Payment.VnPay
{
    public interface IVnPayService
    {
        string CreateVnPayPaymentUrl(OrderDto order, HttpContext context);
        PaymentVnPayResponseModel PaymentExecute(IQueryCollection collections);
    }
}
