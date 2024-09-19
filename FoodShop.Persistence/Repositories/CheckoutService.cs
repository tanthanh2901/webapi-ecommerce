using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Services;
using FoodShop.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FoodShop.Persistence.Repositories
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService; // Updated to use IPaymentService

        public CheckoutService(ICartRepository cartRepository, IOrderRepository orderRepository, IPaymentService paymentService)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _paymentService = paymentService;
        }

        public async Task<Order> ProcessCheckoutAsync(int userId, string paymentMethod)
        {
            // Get the user's cart
            var cartItems = await _cartRepository.GetCartItems(userId);
            if (!cartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty");
            }

            // Calculate total amount
            decimal totalAmount = 0;
            foreach(CartItem item in cartItems)
            {
                totalAmount += item.Product.Price * item.Quantity;
            }

            // Process payment
            bool paymentSuccessful = await _paymentService.ProcessPaymentAsync(totalAmount, paymentMethod);
            if (!paymentSuccessful)
            {
                throw new InvalidOperationException("Payment failed");
            }

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                ShipName = "John Doe",
                ShipAddress = "123 Main St",
                OrderDetail = cartItems.Select(item => new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                }).ToList(),
                Status = Domain.Enum.OrderStatus.Pending
            };

            // Save order
            order = await _orderRepository.CreateOrderAsync(order);

            // Clear the cart
            await _cartRepository.ClearCartAsync(userId);

            return order;
        }
    }
}
