using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Dto;
using FoodShop.Application.Feature.Payment.VnPay;
using FoodShop.Application.Services.Payment.ZaloPay;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace FoodShop.Application.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly ZaloPayConfig _zaloPayConfig;
        private readonly IConfiguration configuration;
        public PaymentService(IOptions<ZaloPayConfig> zaloPayConfig, IConfiguration configuration)
        {
            _zaloPayConfig = zaloPayConfig.Value;
            this.configuration = configuration;
        }
        public async Task<(bool Success, string MessageOrLink)> CreateZaloPaymentLinkAsync(PaymentDto paymentDto)
        {
            var zalopay_Params = new Dictionary<string, object>
            {
                { "appid", _zaloPayConfig.AppId },
                { "apptransid", paymentDto.TransactionId},
                { "apptime", DateTimeOffset.Now.ToUnixTimeMilliseconds() },
                { "appuser", _zaloPayConfig.AppUser },
                { "amount", (long)paymentDto.Amount },
                { "description", $"Thanh toan don hang #{paymentDto.OrderId}" },
                { "bankcode", "zalopayapp" }
            };

            var item = "[{\"itemid\":\"knb\",\"itemname\":\"kim nguyen bao\",\"itemprice\":198400,\"itemquantity\":1}]";
            zalopay_Params.Add("item", item);

            var embeddata = new Dictionary<string, string>
            {
                { "merchantinfo", "eshop123" },
                { "promotioninfo", "" },
                { "redirecturl", _zaloPayConfig.RedirectUrl },
                { "columninfo", JsonConvert.SerializeObject(new { store_name = "E-Shop" }) }
            };
            zalopay_Params.Add("embeddata", JsonConvert.SerializeObject(embeddata));

            string data = $"{zalopay_Params["appid"]}|{zalopay_Params["apptransid"]}|{zalopay_Params["appuser"]}|{zalopay_Params["amount"]}|{zalopay_Params["apptime"]}|{zalopay_Params["embeddata"]}|{zalopay_Params["item"]}";
            zalopay_Params.Add("mac", ComputeHmacSha256(_zaloPayConfig.Key1, data));

            using var client = new HttpClient();
            var formContent = new FormUrlEncodedContent(zalopay_Params.ToDictionary(k => k.Key, v => v.Value.ToString()));

            try
            {
                var response = await client.PostAsync(_zaloPayConfig.PaymentUrl, formContent);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ZaloPayResponse>(jsonResponse);

                if (result.returnCode == 1)
                {
                    return (true, result.orderUrl);
                }
                else
                {
                    return (false, result.returnMessage);
                }
            }
            catch (Exception ex)
            {
                return (false, "error");
            }
        }
        public async Task<(bool Success, string StatusMessage)> CheckZaloPaymentStatusAsync(string appTransId)
        {
            var requestParams = new Dictionary<string, object>
            {
                {"appid", _zaloPayConfig.AppId},
                {"apptransid", appTransId},
            };

            var dataToSign = $"{_zaloPayConfig.AppId}|{appTransId}|{_zaloPayConfig.Key1}";
            requestParams.Add("mac", ComputeHmacSha256(_zaloPayConfig.Key1, dataToSign));

            using var client = new HttpClient();
            try
            {
                var formContent = new FormUrlEncodedContent(requestParams.ToDictionary(k => k.Key, v => v.Value.ToString()));

                var response = await client.PostAsync(_zaloPayConfig.QueryUrl, formContent);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<CheckStatusResponse>(responseContent);

                    if (responseData.returnCode == 1)
                    {
                        return (true, "Payment Success: " + responseData.returnMessage);
                    }
                    else
                    {
                        return (false, "Payment Failed: " + responseData.returnMessage);
                    }
                }
                else
                {
                    return (false, "Error occurred while checking payment status");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }    
        private string ComputeHmacSha256(string key, string rawData)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public string CreateVnPayPaymentUrl(OrderDto order, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", configuration["VnPay:vnp_TmnCode"]);

            vnpay.AddRequestData("vnp_Amount", ((int)(order.TotalAmount*100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));

            vnpay.AddRequestData("vnp_BankCode", "");

            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", vnpay.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang: " + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", configuration["VnPay:vnp_Returnurl"]);
            vnpay.AddRequestData("vnp_ExpireDate", order.OrderDate.AddDays(1).ToString("yyyyMMddHHmmss"));

            vnpay.AddRequestData("vnp_TxnRef", tick); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            //vnpay.AddRequestData("vnp_SecureHash", configuration["VnPay:vnp_Returnurl"]);

            string paymentUrl = vnpay.CreateRequestUrl(configuration["VnPay:vnp_Url"], configuration["VnPay:vnp_HashSecret"]);
            return paymentUrl;
        }

        public PaymentVnPayResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, configuration["VnPay:vnp_HashSecret"]);

            return response;
        }
    }
}
