using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Feature.Account.Register
{
    public class RegisterResponse
    {
        public string Message { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}
