using AutoMapper;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Dto;
using FoodShop.Application.Feature.Payment.Commands;
using FoodShop.Application.Services;
using FoodShop.Application.Services.Payment;
using FoodShop.Domain.Entities;

namespace FoodShop.Persistence.Repositories
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository userRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IMapper mapper;
        private readonly INotificationRepository notificationRepository;
        private readonly CurrencyExchangeService currencyExchangeService;

        public CheckoutService(
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            CurrencyExchangeService currencyExchangeService,
            IPaymentRepository paymentRepository,
            IMapper mapper,
            INotificationRepository notificationRepository)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            this.userRepository = userRepository;
            this.currencyExchangeService = currencyExchangeService;
            this.paymentRepository = paymentRepository;
            this.mapper = mapper;
            this.notificationRepository = notificationRepository;
        }

        public async Task<OrderDto> ProcessCheckoutAsync(PlaceOrderRequest placeOrderRequest)
        {
            if (placeOrderRequest.UserId == null)
            {
                throw new ArgumentNullException(nameof(placeOrderRequest.UserId), "User ID is required for notification.");
            }

            // Get the user's cart
            var cartItems = await _cartRepository.GetCartItems(placeOrderRequest.UserId);
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

            // Create order
            var order = new Order
            {
                UserId = placeOrderRequest.UserId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                ShipAddress = placeOrderRequest.ShipAddress,
                ShipName = placeOrderRequest.ShipName,
                PhoneNumber = placeOrderRequest.PhoneNumber,
                OrderDetail = cartItems.Select(item => new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                }).ToList(),
                Status = Domain.Enum.OrderStatus.Pending
            };

            order = await _orderRepository.CreateOrderAsync(order);

            var payment = new PaymentDto
            {
                Amount = totalAmount,
                MethodId = placeOrderRequest.PaymentMethodId,
                OrderId = order.OrderId,
                PaymentDate = order.OrderDate,
                TransactionId = GenerateAppTransId(),
                Status = order.Status.ToString()
            };
            payment.OrderId = order.OrderId;

            await paymentRepository.AddPayment(payment);

            return mapper.Map<OrderDto>(order);          

        }

        public async Task UpdateDbWhenPaymentSuccess(OrderDto orderDto)
        {
            await _cartRepository.ClearCartAsync(orderDto.UserId);

            // Send notification to admin (assumed admin ID is 1)
            await notificationRepository
                .AddNotificationAsync(1, $"A new order #{orderDto.OrderId} has been placed.");
        }

        private string GenerateAppTransId()
        {
            return DateTime.UtcNow.ToString("yyMMdd") + "_" + new Random().Next(100000, 999999).ToString();
        }
    }
}
