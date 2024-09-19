using FoodShop.Application.Dto;
using MediatR;

namespace FoodShop.Application.Feature.Categories.Queries.GetProductDetails
{
    public class GetCategoryDetailsQuery : IRequest<CategoryDto>
    {
        public int CategoryId;
    }
}
