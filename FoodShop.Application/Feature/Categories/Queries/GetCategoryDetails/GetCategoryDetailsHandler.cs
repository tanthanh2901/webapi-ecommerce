using AutoMapper;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Dto;
using FoodShop.Application.Entities;
using MediatR;

namespace FoodShop.Application.Feature.Categories.Queries.GetProductDetails
{
    public class GetCategoryDetailsHandler : IRequestHandler<GetCategoryDetailsQuery, CategoryDto>
    {
        private readonly IRepository<Category> categoryRepository;
        private readonly IMapper mapper;

        public GetCategoryDetailsHandler(IMapper mapper, IRepository<Category> categoryRepository)
        {
            this.mapper = mapper;
            this.categoryRepository = categoryRepository;
        }

        public async Task<CategoryDto> Handle(GetCategoryDetailsQuery request, CancellationToken cancellationToken)
        {
            var category = await categoryRepository.GetByIdAsync(request.CategoryId);
            var categoryDto = mapper.Map<CategoryDto>(category);


            return categoryDto;
        }
    }
}
