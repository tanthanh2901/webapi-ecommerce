using AutoMapper;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Domain.Entities;
using MediatR;

namespace FoodShop.Application.Feature.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
    {
        private readonly IRepository<Category> categoryRepository;
        private readonly IMapper mapper;

        public CreateCategoryCommandHandler(IRepository<Category> categoryRepository, IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var newCategory = mapper.Map<Category>(request);
            await categoryRepository.AddAsync(newCategory);

            return newCategory.CategoryId;
        }
    }
}
