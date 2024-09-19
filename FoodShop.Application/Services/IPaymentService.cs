namespace FoodShop.Application.Services
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(decimal amount, string paymentMethod);
    }

    public class PaymentService : IPaymentService
    {
        public async Task<bool> ProcessPaymentAsync(decimal amount, string paymentMethod)
        {
            // Simulate payment processing
            await Task.Delay(1000);
            return true; // Always return true for this example
        }
    }
}
