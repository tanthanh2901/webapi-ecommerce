using AutoMapper;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Dto;
using FoodShop.Domain.Entities;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FoodShop.Application.Feature.Products.Queries.GetAllProducts
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, PaginatedResult<List<ProductListDto>>>
    {
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;

        public GetAllProductQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            this.productRepository = productRepository;
            this.mapper = mapper;
        }

        public async Task<PaginatedResult<List<ProductListDto>>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var allProducts = await productRepository.GetAllAsync();

            var allproductsDto = mapper.Map<List<ProductListDto>>(allProducts);

            var pagedProducts = allproductsDto
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var totalItems = allproductsDto.Count;

            return new PaginatedResult<List<ProductListDto>>(
                pagedProducts, totalItems, request.PageNumber, request.PageSize);
        }
        
    }
}
