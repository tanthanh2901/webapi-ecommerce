using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Dto;
using FoodShop.Application.Services;
using FoodShop.Application.Services.Payment;
using FoodShop.Domain.Entities;

namespace FoodShop.Persistence.Repositories
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService; // Updated to use IPaymentService
        private readonly IUserRepository userRepository;
        private readonly IPaymentMethodRepository paymentMethodRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly CurrencyExchangeService currencyExchangeService;
        private readonly FoodShopDbContext dbContext;


        public CheckoutService(ICartRepository cartRepository, IOrderRepository orderRepository, IPaymentService paymentService, IUserRepository userRepository, IPaymentMethodRepository paymentMethodRepository, CurrencyExchangeService currencyExchangeService, FoodShopDbContext dbContext, IPaymentRepository paymentRepository)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            this.userRepository = userRepository;
            this.paymentMethodRepository = paymentMethodRepository;
            this.currencyExchangeService = currencyExchangeService;
            this.dbContext = dbContext;
            this.paymentRepository = paymentRepository;
        }

        public async Task<(Order order, string orderLink)> ProcessCheckoutAsync(int userId, int paymentMethodId)
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

            var exchangeRate = 25000;//await currencyExchangeService.GetUsdToVndRateAsync();
            totalAmount = totalAmount * exchangeRate;

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

            var paymentMethod = await paymentMethodRepository.GetByIdAsync(paymentMethodId);
            // Process payment
            var payment = new PaymentDto
            {
                Amount = totalAmount,
                MethodId = paymentMethodId,
                OrderId = order.OrderId,
                PaymentDate = order.OrderDate,
                TransactionId = GenerateAppTransId(),
                Status = order.Status.ToString()
            };

            switch (paymentMethod.MethodName)
            {
                case "cod":
                    order = await _orderRepository.CreateOrderAsync(order);
                    payment.OrderId = order.OrderId;
                    await paymentRepository.AddPayment(payment);

                    await _cartRepository.ClearCartAsync(userId);
                    return (order, "Check out successfully");
                case "zalopay":
                    order = await _orderRepository.CreateOrderAsync(order);

                    payment.OrderId = order.OrderId;
                    await paymentRepository.AddPayment(payment);

                    var (success, result) = await _paymentService.CreateZaloPaymentLinkAsync(payment);
                    
                    if (!success)
                    {
                        throw new InvalidOperationException(result);
                    }
                    return (order, result);             
            }
            //var (statusSuccess, paymentStatusMessage) = await _paymentService.CheckZaloPaymentStatusAsync(GenerateAppTransId(order));
            //if(!statusSuccess)
            //{
            //    order.Status = Domain.Enum.OrderStatus.Canceled;
            //    await _orderRepository.UpdateOrderStatusAsync(order.OrderId, Domain.Enum.OrderStatus.Canceled);
            //    throw new InvalidOperationException($"Payment failed: {paymentStatusMessage}");
            //}

            //order.Status = Domain.Enum.OrderStatus.Confirmed;
            //await _orderRepository.UpdateOrderStatusAsync(order.OrderId, Domain.Enum.OrderStatus.Confirmed);

            throw new InvalidOperationException("Invalid payment method");
        }

        private string GenerateAppTransId()
        {
            return DateTime.UtcNow.ToString("yyMMdd") + "_" + new Random().Next(100000, 999999).ToString();
        }
    }
}
