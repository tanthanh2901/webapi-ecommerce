//using AutoMapper;
//using FoodShop.Application.Contract.Persistence;
//using FoodShop.Application.Dto;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FoodShop.Application.Feature.Cart.Queries.GetCart
//{
//    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
//    {
//        private readonly ICartRepository cartRepository;
//        private readonly IMapper mapper;

//        public GetCartQueryHandler(ICartRepository cartRepository, IMapper mapper)
//        {
//            this.cartRepository = cartRepository;
//            this.mapper = mapper;
//        }

//        public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
//        {
//            var cart = await cartRepository.GetCartByUserId(request.UserId);

//            var cartToReturn = mapper.Map<CartDto>(cart);
//            return cartToReturn; 
               
//        }
//    }
//}
