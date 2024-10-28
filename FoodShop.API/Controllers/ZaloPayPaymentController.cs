using FoodShop.Application.Services.Payment;
using FoodShop.Application.Services.Payment.ZaloPay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace FoodShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZaloPayPaymentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ZaloPayConfig _zaloPayConfig;
        private readonly IPaymentService paymentService;


        public ZaloPayPaymentController(IHttpClientFactory httpClientFactory, IOptions<ZaloPayConfig> zaloPayConfig, IPaymentService paymentService)
        {
            _httpClientFactory = httpClientFactory;
            _zaloPayConfig = zaloPayConfig.Value;
            this.paymentService = paymentService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreatePayment([FromQuery] string appuser, [FromQuery] long amount, [FromQuery] long order_id)
        {
            var zalopay_Params = new Dictionary<string, object>
            {
                { "appid", _zaloPayConfig.AppId },
                { "apptransid", GenerateAppTransId() },
                { "apptime", DateTimeOffset.Now.ToUnixTimeMilliseconds() },
                { "appuser", appuser },
                { "amount", amount },
                { "description", $"Thanh toan don hang #{order_id}" },
                { "bankcode", "" }
            };

            // Item list (you can dynamically generate this if necessary)
            var item = "[{\"itemid\":\"knb\",\"itemname\":\"kim nguyen bao\",\"itemprice\":198400,\"itemquantity\":1}]";
            zalopay_Params.Add("item", item);

            // Embed data
            var embeddata = new Dictionary<string, string>
            {
                { "merchantinfo", "eshop123" },
                { "promotioninfo", "" },
                { "redirecturl", _zaloPayConfig.RedirectUrl },
                { "columninfo", JsonConvert.SerializeObject(new { store_name = "E-Shop" }) }
            };
            zalopay_Params.Add("embeddata", JsonConvert.SerializeObject(embeddata));

            // Generate MAC
            string data = $"{zalopay_Params["appid"]}|{zalopay_Params["apptransid"]}|{zalopay_Params["appuser"]}|{zalopay_Params["amount"]}|{zalopay_Params["apptime"]}|{zalopay_Params["embeddata"]}|{zalopay_Params["item"]}";
            zalopay_Params.Add("mac", ComputeHmacSha256(_zaloPayConfig.Key1, data));

            // HTTP POST to ZaloPay
            using var client = _httpClientFactory.CreateClient();
            var formContent = new FormUrlEncodedContent(zalopay_Params.ToDictionary(k => k.Key, v => v.Value.ToString()));

            try
            {
                var response = await client.PostAsync(_zaloPayConfig.PaymentUrl, formContent);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);

                return Ok(new
                {
                    returnmessage = result["returnmessage"],
                    orderurl = result["orderurl"],
                    returncode = result["returncode"],
                    zptranstoken = result["zptranstoken"]
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { returnmessage = $"Error: {ex.Message}", returncode = -1 });
            }
        }
        [HttpPost("create-order-paymentservice")]
        public async Task<IActionResult> CreatePaymentService(long amount, string descripton, string order_id)
        {
            var (success, link) = await paymentService.CreateZaloPaymentLinkAsync(amount, descripton, order_id);
            if(success)
            {
                return Ok("order-link: " + link);
            }
            else
            {
                return BadRequest(link);
            }
        }

        private string GenerateAppTransId()
        {
            return DateTime.UtcNow.ToString("yyMMdd") + "_" + new Random().Next(100000, 999999).ToString();
        }
        private string ComputeHmacSha256(string key, string rawData)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
