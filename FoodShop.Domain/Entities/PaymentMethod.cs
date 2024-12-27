using System.ComponentModel.DataAnnotations;

namespace FoodShop.Domain.Entities
{
    public class PaymentMethod
    {
        [Key]
        public int MethodId { get; set; }

        [Required]
        [MaxLength(20)]
        public string MethodName { get; set; }

        // Navigation property
        public ICollection<Payment> Payments { get; set; }
    }
}
