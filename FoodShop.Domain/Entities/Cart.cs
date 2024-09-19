using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodShop.Domain.Entities
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}