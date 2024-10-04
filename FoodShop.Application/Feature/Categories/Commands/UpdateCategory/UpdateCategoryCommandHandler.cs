using AutoMapper;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Domain.Entities;
using MediatR;

namespace FoodShop.Application.Feature.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly IRepository<Category> categoryRepository;
        private readonly IMapper mapper;

        public UpdateCategoryCommandHandler(IRepository<Category> categoryRepository, IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }
        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryToUpdate = await categoryRepository.GetByIdAsync(request.CategoryId);
            mapper.Map(request, categoryToUpdate, typeof(UpdateCategoryCommand), typeof(Category));

            await categoryRepository.UpdateAsync(categoryToUpdate);
        }
    }
}
