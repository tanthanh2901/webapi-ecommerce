namespace FoodShop.Application.Feature.Payment.Commands
{
    public class PlaceOrderRequest
    {
        public int UserId { get; set; }
        public string ShipAddress { get; set; } = string.Empty;
        public string ShipName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int PaymentMethodId { get; set; }
    }
}
