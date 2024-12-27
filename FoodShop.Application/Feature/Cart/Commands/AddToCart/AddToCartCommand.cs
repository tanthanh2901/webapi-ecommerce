using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Feature.Cart.Commands.AddToCart
{
    public class AddToCartCommand : IRequest<bool>
    {
        public int ProductId;
        public int userId;
    }
}
