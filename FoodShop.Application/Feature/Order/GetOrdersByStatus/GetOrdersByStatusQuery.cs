using AutoMapper;
using FoodShop.Application.Dto;
using FoodShop.Domain.Enum;
using FoodShop.Persistence.Repositories;
using MediatR;

namespace FoodShop.Application.Feature.Order.GetOrdersByStatus
{
    public class GetOrdersByStatusQuery : IRequest<List<OrderDto>>
    {
        public int UserId;
        public OrderStatus OrderStatus;
    }

    public class GetOrdersByStatusQueryHandler : IRequestHandler<GetOrdersByStatusQuery, List<OrderDto>>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;

        public GetOrdersByStatusQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
        }

        public async Task<List<OrderDto>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetOrderByStatus(request.UserId, request.OrderStatus);
            return mapper.Map<List<OrderDto>>(order);
        }
    }
}
