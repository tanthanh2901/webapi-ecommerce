//using AutoMapper;
//using FoodShop.Application.Contract.Persistence;
//using FoodShop.Application.Dto;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FoodShop.Application.Feature.Cart.Commands.UpdateQuantityCartItem
//{
//    public class UpdateQuantityCartItemCommandHandler : IRequestHandler<UpdateQuantityCartItemCommand, List<CartItemDto>>
//    {
//        private readonly ICartRepository cartRepository;
//        private readonly IMapper mapper;

//        public UpdateQuantityCartItemCommandHandler(ICartRepository cartRepository, IMapper mapper)
//        {
//            this.cartRepository = cartRepository;
//            this.mapper = mapper;
//        }

//        public async Task<List<CartItemDto>> Handle(UpdateQuantityCartItemCommand request, CancellationToken cancellationToken)
//        {
//            var cart = await cartRepository.UpdateCartItem(request.UserId, request.CartItemId, request.Quantity);

//            var cartToReturn = mapper.Map<List<CartItemDto>>(cart);
//            return cartToReturn;
//        }
//    }
//}
