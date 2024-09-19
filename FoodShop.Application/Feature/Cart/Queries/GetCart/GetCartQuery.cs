using FoodShop.Application.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Feature.Cart.Queries.GetCart
{
    public class GetCartQuery : IRequest<CartDto> 
    {
        public int UserId;
    }
}
