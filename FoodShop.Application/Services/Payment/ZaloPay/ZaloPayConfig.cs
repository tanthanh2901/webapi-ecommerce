namespace FoodShop.Application.Services.Payment.ZaloPay
{
    public class ZaloPayConfig
    {
        public static string ConfigName => "ZaloPay";
        public string AppUser { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty; 
        public string QueryUrl { get; set; } = string.Empty;
        public string IpUrl { get; set; } = string.Empty;
        public string AppId { get; set; } = string.Empty;
        public string Key1 { get; set; } = string.Empty;
        public string Key2 { get; set; } = string.Empty;
    }
}
