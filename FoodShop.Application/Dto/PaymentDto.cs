namespace FoodShop.Application.Dto
{
    public class PaymentDto
    {
        public int OrderId { get; set; }
        public int MethodId { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; }

    }
}
