using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Dto;
using FoodShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly FoodShopDbContext dbContext;

        public PaymentRepository(FoodShopDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> AddPayment(PaymentDto paymentDto)
        {
            var payment = new Payment();
            payment.Amount = paymentDto.Amount;
            payment.MethodId = paymentDto.MethodId;
            payment.OrderId = paymentDto.OrderId;
            payment.TransactionId = paymentDto.TransactionId;
            payment.PaymentDate = paymentDto.PaymentDate;
            payment.Status = paymentDto.Status;

            dbContext.Payments.Add(payment);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<PaymentDto> GetPaymentByTransId(string transId)
        {
            var payment = await dbContext.Payments.FirstOrDefaultAsync(p => p.TransactionId == transId);
            return new PaymentDto
            {
                Amount = payment.Amount,
                MethodId = payment.MethodId,
                OrderId = payment.OrderId,
                TransactionId = payment.TransactionId,
                PaymentDate = payment.PaymentDate,
                Status = payment.Status
            };
        }
    }
}
