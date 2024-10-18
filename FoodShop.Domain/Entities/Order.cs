using FoodShop.Domain.Enum;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodShop.Domain.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string PhoneNumber { get; set; }

        public AppUser User { get; set; }

        public ICollection<OrderDetail> OrderDetail { get; set; }
        public OrderStatus Status { get; set; }
    }
}