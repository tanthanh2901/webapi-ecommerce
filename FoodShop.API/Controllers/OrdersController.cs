using FoodShop.Application.Dto;
using FoodShop.Application.Feature.Order.GetAllOrders;
using FoodShop.Application.Feature.Order.GetOrder;
using FoodShop.Application.Feature.Order.GetOrdersByStatus;
using FoodShop.Domain.Entities;
using FoodShop.Domain.Enum;
using FoodShop.Persistence.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FoodShop.API.Controllers
{
    [Route("orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator mediatR;
        private readonly IOrderRepository orderRepository;
        private readonly UserManager<AppUser> _userManager;

        public OrdersController(UserManager<AppUser> userManager, IMediator mediatR, IOrderRepository orderRepository)
        {
            _userManager = userManager;
            this.mediatR = mediatR;
            this.orderRepository = orderRepository;
        }

        private async Task<int> GetUserId()
        {
            var user = await _userManager.GetUserAsync(User);
            return (int)(user?.Id);
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
        {
            var userId = await GetUserId();
            if (userId == null) return Unauthorized();

            //var orders = await mediatR.Send(new GetAllOrdersQuery() { UserId = userId});
            var orders =  await orderRepository.GetAllOrders(userId);
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            //var orders = await mediatR.Send(new GetOrderQuery() { OrderId = orderId});
            var order = await orderRepository.GetOrderByIdAsync(orderId);

            return Ok(order);
        }
        

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var userId = await GetUserId();
            if (userId == null) return Unauthorized();

            //var pendingOrders = await orderRepository.GetOrderByStatus(userId, OrderStatus.Pending);
            var pendingOrders = await mediatR.Send(new GetOrdersByStatusQuery() { UserId = userId, OrderStatus = OrderStatus.Pending });
            return Ok(pendingOrders);
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> GetConfirmOrders()
        {
            var userId = await GetUserId();
            if (userId == null) return Unauthorized();

            var confirmOrders = await mediatR.Send(new GetOrdersByStatusQuery() { UserId = userId, OrderStatus = OrderStatus.Confirmed });
            return Ok(confirmOrders);
        }

        [HttpGet("shipping")]
        public async Task<IActionResult> GetShippingOrders()
        {
            var userId = await GetUserId();
            if (userId == null) return Unauthorized();

            var shippingOrders = await mediatR.Send(new GetOrdersByStatusQuery() { UserId = userId, OrderStatus = OrderStatus.Shipping });
            return Ok(shippingOrders);
        }

        [HttpGet("delivered")]
        public async Task<IActionResult> GetDeliveredOrders()
        {
            var userId = await GetUserId();
            if (userId == null) return Unauthorized();

            var deliveredOrders = await mediatR.Send(new GetOrdersByStatusQuery() { UserId = userId, OrderStatus = OrderStatus.Delivered });
            return Ok(deliveredOrders);
        }

        [HttpGet("canceled")]
        public async Task<IActionResult> GetCanceledOrders()
        {
            var userId = await GetUserId();
            if (userId == null) return Unauthorized();

            var CanceledOrders = await mediatR.Send(new GetOrdersByStatusQuery() { UserId = userId, OrderStatus = OrderStatus.Canceled });
            return Ok(CanceledOrders);
        }
    }
}
