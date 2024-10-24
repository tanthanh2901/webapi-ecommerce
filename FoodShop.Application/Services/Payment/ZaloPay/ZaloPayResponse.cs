namespace FoodShop.Application.Services.Payment.ZaloPay
{
    public class ZaloPayResponse
    {
        public int returnCode { get; set; }
        public string returnMessage { get; set; } = string.Empty;
        public string orderUrl { get; set; } = string.Empty;
    }
}
