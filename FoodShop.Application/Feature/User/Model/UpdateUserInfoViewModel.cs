using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Feature.User.Model
{
    public class UpdateUserInfoViewModel
    {
        public string ShipName { get; set; } = string.Empty;
        public string ShipAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
    }
}
