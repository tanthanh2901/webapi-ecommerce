using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Persistence.Repositories
{
    public interface IPaymentRepository
    {
        Task<bool> ProcessPaymentAsync(decimal amount, string paymentMethod);
        public class PaymentRepository : IPaymentRepository
        {
            public async Task<bool> ProcessPaymentAsync(decimal amount, string paymentMethod)
            {
                // Simulate payment processing
                await Task.Delay(1000);
                return true; // Always return true for this example
            }
        }  

    }
}
