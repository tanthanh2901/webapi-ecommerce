using FoodShop.Application.Dto;
using MediatR;

namespace FoodShop.Application.Feature.Products.Queries.GetAllProducts
{
    public class GetAllProductQuery : IRequest<PaginatedResult<List<ProductListDto>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
