using AutoMapper;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Domain.Entities;
using FoodShop.Application.Feature.Products.Commands.DeleteProduct;
using MediatR;

namespace FoodShop.Application.Feature.Products.Commands.CreateProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IRepository<Product> repository;
        private readonly IMapper mapper;

        public DeleteProductCommandHandler(IRepository<Product> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var productToDelete = await repository.GetByIdAsync(request.ProductId);
            await repository.DeleteAsync(productToDelete);
        }
    }
}
