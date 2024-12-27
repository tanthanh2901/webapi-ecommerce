using FoodShop.Application.Dto;

namespace FoodShop.Application.Contract.Persistence
{
    public interface IPaymentRepository
    {
        Task<bool> AddPayment(PaymentDto paymentDto);
        Task<PaymentDto> GetPaymentByTransId(string transId);
    }
}
