using AutoMapper;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Dto;
using FoodShop.Application.Entities;
using MediatR;

namespace FoodShop.Application.Feature.Products.Queries.GetAllProducts
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, List<ProductListDto>>
    {
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;

        public GetAllProductQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            this.productRepository = productRepository;
            this.mapper = mapper;
        }

        public async Task<List<ProductListDto>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var allProducts = await productRepository.GetAllAsync();
            return mapper.Map<List<ProductListDto>>(allProducts);
        }
    }
}
