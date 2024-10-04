using AutoMapper;
using FoodShop.Application.Dto;
using FoodShop.Persistence.Repositories;
using MediatR;

namespace FoodShop.Application.Feature.Order.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderDto>>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;

        public GetAllOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
        }

        async Task<List<OrderDto>> IRequestHandler<GetAllOrdersQuery, List<OrderDto>>.Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await orderRepository.GetAllOrders(request.UserId);
            return mapper.Map<List<OrderDto>>(orders);
        }
    }
}
