using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FoodShop.Domain.Entities
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        [JsonIgnore]
        public Cart Cart { get; set; } // Navigation property to the cart
        public int ProductId { get; set; } // ID of the product
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
