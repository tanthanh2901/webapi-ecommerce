using AutoMapper;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Dto;
using FoodShop.Domain.Entities;
using MediatR;

namespace FoodShop.Application.Feature.Products.Queries.GetProductDetails
{
    public class GetProductDetailsHandler : IRequestHandler<GetProductDetailsQuery, ProductDetailsDto>
    {
        private readonly IProductRepository productRepository;
        private readonly IRepository<Category> categoryRepository;
        private readonly IMapper mapper;

        public GetProductDetailsHandler(IProductRepository productRepository, IMapper mapper, IRepository<Category> categoryRepository)
        {
            this.productRepository = productRepository;
            this.mapper = mapper;
            this.categoryRepository = categoryRepository;
        }

        public async Task<ProductDetailsDto> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetByIdAsync(request.ProductId);
            var productDto = mapper.Map<ProductDetailsDto>(product);

            var category = await categoryRepository.GetByIdAsync(product.CategoryId);
            productDto.Category = mapper.Map<CategoryDto>(category);

            return productDto;
        }
    }
}
