﻿using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Services;
using FoodShop.Domain.Entities;

namespace FoodShop.Persistence.Repositories
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService; // Updated to use IPaymentService
        private readonly IUserRepository userRepository;


        public CheckoutService(ICartRepository cartRepository, IOrderRepository orderRepository, IPaymentService paymentService, IUserRepository userRepository)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            this.userRepository = userRepository;
        }

        public async Task<(Order order, string orderLink)> ProcessCheckoutAsync(int userId, string paymentMethod)
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
            var (success, result) = await _paymentService.CreatePaymentLinkAsync((long)totalAmount, paymentMethod, "Orderid");
            if (!success)
            {
                throw new InvalidOperationException(result);
            }
            Console.WriteLine(result);
            var user = await userRepository.GetUserInfo(userId);
            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                ShipAddress = user.ShipAddress,
                ShipName = user.ShipName,
                PhoneNumber = user.PhoneNumber,
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

            //var (statusSuccess, paymentStatusMessage) = await _paymentService.CheckPaymentStatusAsync(GenerateAppTransId(order));
            //if(!statusSuccess)
            //{
            //    order.Status = Domain.Enum.OrderStatus.Canceled;
            //    await _orderRepository.UpdateOrderStatusAsync(order.OrderId, Domain.Enum.OrderStatus.Canceled);
            //    throw new InvalidOperationException($"Payment failed: {paymentStatusMessage}");
            //}

            //order.Status = Domain.Enum.OrderStatus.Confirmed;
            //await _orderRepository.UpdateOrderStatusAsync(order.OrderId, Domain.Enum.OrderStatus.Confirmed);

            return (order, result);
        }

        private string GenerateAppTransId(Order order)
        {
            return order.OrderDate.ToString("yyMMdd") + "_" + new Random().Next(100000, 999999).ToString();
        }
    }
}
