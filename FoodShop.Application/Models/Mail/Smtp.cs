namespace FoodShop.Application.Models.Mail
{
    public class Smtp
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; } = string.Empty!;
        public string SenderPassword { get; set; } = string.Empty!;
    }
}
