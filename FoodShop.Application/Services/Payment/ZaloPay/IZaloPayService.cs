using FoodShop.Application.Dto;

namespace FoodShop.Application.Services.Payment.ZaloPay
{
    public interface IZaloPayService
    {
        Task<(bool Success, string MessageOrLink)> CreateZaloPaymentLinkAsync(PaymentDto paymentDto);
        Task<(bool Success, string StatusMessage)> CheckZaloPaymentStatusAsync(string appTransId);

    }
}
