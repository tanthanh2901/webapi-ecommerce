using FoodShop.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Domain.Entities
{
    public class CartOrderInfo
    {
        public List<CartItem> products;
        //public decimal freight;
        public decimal totalPrice;
    }
}
