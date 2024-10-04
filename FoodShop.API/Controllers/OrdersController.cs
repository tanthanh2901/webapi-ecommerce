using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Feature.Order.GetAllOrders;
using FoodShop.Application.Feature.Order.GetOrder;
using FoodShop.Domain.Entities;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly IOrderRepository orderRepository;

        public OrdersController(UserManager<AppUser> userManager, IOrderRepository orderRepository, IMediator mediatR)
        {
            _userManager = userManager;
            this.orderRepository = orderRepository;
            this.mediatR = mediatR;
        }

        private async Task<int> GetUserId()
        {
            var user = await _userManager.GetUserAsync(User);
            return (int)(user?.Id);
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetAllOrders()
        {
            var userId = await GetUserId();
            if (userId == null) return Unauthorized();

            var orders = await mediatR.Send(new GetAllOrdersQuery() { UserId = userId});
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var orders = await mediatR.Send(new GetOrderQuery() { OrderId = orderId});
            return Ok(orders);
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

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
    }
}
