namespace FoodShop.Application.Dto
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
