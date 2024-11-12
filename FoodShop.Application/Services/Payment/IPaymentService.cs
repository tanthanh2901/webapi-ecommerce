using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Dto;
using FoodShop.Application.Services.Payment.ZaloPay;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FoodShop.Application.Services.Payment
{
    public interface IPaymentService
    {
        Task<(bool Success, string MessageOrLink)> CreateZaloPaymentLinkAsync(PaymentDto paymentDto);
        Task<(bool Success, string StatusMessage)> CheckZaloPaymentStatusAsync(string appTransId);
    }

    public class PaymentService : IPaymentService
    {
        private readonly ZaloPayConfig _zaloPayConfig; 
        private readonly IPaymentRepository paymentRepository;

        public PaymentService(IOptions<ZaloPayConfig> zaloPayConfig, IPaymentRepository paymentRepository = null)
        {
            _zaloPayConfig = zaloPayConfig.Value;
            this.paymentRepository = paymentRepository;
        }
        public async Task<(bool Success, string MessageOrLink)> CreateZaloPaymentLinkAsync(PaymentDto paymentDto) {
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

       

        private string ComputeHmacSha256(string key, string rawData)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
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
    }
}
