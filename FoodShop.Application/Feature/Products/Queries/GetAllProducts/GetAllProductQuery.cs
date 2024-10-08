using FoodShop.Application.Dto;
using FoodShop.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Feature.Products.Queries.GetAllProducts
{
    public class GetAllProductQuery : IRequest<PaginatedResult<List<ProductListDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
