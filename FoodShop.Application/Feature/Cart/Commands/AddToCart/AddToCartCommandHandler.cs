//using FoodShop.Application.Contract.Persistence;
//using FoodShop.Application.Entities;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FoodShop.Application.Feature.Cart.Commands.AddToCart
//{
//    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, bool>
//    {
//        private readonly ICartRepository cartRepository;
//        private readonly IRepository<Product> productRepository;


//        public AddToCartCommandHandler(ICartRepository cartRepository, IRepository<Product> productRepository)
//        {
//            this.cartRepository = cartRepository;
//            this.productRepository = productRepository;
//        }

//        public async Task<bool> Handle(AddToCartCommand request, CancellationToken cancellationToken)
//        {
//            var product = await productRepository.GetByIdAsync(request.ProductId);
//            if(product  != null)
//            {
//                await cartRepository.AddToCart(request.userId, product);
//                return true;
//            }
//            return false;
//        }
//    }
//}
