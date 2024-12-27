using FoodShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Dto
{
    public class CartDto
    {
        //public UserDto User { get; set; }
        public ICollection<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }
}
