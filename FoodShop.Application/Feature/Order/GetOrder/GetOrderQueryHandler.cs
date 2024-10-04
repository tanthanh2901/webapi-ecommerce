using AutoMapper;
using FoodShop.Application.Dto;
using FoodShop.Persistence.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Feature.Order.GetOrder
{
    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDto>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;

        public GetOrderQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
        }

        async Task<OrderDto> IRequestHandler<GetOrderQuery, OrderDto>.Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetOrderByIdAsync(request.OrderId);
            return mapper.Map<OrderDto>(order);
        }
    }
}
