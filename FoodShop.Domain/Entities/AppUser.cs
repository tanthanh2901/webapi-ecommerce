using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FoodShop.Domain.Entities
{
    public class AppUser : IdentityUser<int> 
    {

        public Cart Cart { get; set; }
        //[ForeignKey]
        //public int CartId { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

    }
}
