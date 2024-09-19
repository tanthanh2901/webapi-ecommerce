using System.ComponentModel.DataAnnotations;

namespace FoodShop.Application.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}