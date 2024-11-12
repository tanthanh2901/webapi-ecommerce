using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodShop.Domain.Entities
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [ForeignKey(nameof(AppUser))]
        public int AppUserId { get; set; } // Rename UserId to AppUserId
        public AppUser AppUser { get; set; }

        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }

}
