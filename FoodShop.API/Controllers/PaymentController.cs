using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Feature.Payment.Commands;
using FoodShop.Application.Services.Payment;
using FoodShop.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodShop.API.Controllers
{
    [Route("payment")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IMediator mediator;
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService, IMediator mediator)
        {
            this.paymentService = paymentService;
            this.mediator = mediator;
        }

        [HttpPost("placeOrder")]
        public async Task<IActionResult> CreatePaymentUrl(PlaceOrderRequest placeOrderRequest)
        {
            try
            {
                var result = await mediator.Send(new CreatePaymentCommand
                {
                    PlaceOrderRequest = placeOrderRequest
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

        [HttpGet("vnpayCallback")]
        public IActionResult VnPayCallback()
        {
            var response = paymentService.PaymentExecute(Request.Query);
            if (response.Success)
            {
                //checkoutService.UpdateDbWhenPaymentSuccess();
                return Json(new { message = "Payment successful" });
            }
            return Json(new { message = "Payment failed or verification failed" });
        }
    }
}


