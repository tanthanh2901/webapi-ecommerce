using FoodShop.Application.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Feature.Order.GetAllOrders
{
    public class GetAllOrdersQuery : IRequest<List<OrderDto>>
    {
        public int UserId;
    }
}
