using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodShop.Domain.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [ForeignKey("PaymentMethod")]
        public int MethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string TransactionId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }
    }

}
