using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Domain.Entities
{
    public class CartProduct
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        private bool IsSelected;
    }
}
