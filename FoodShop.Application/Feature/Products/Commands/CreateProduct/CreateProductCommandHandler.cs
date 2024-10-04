using AutoMapper;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Domain.Entities;
using MediatR;

namespace FoodShop.Application.Feature.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IRepository<Product> repository;
        private readonly IMapper mapper;

        public CreateProductCommandHandler(IRepository<Product> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = mapper.Map<Product>(request);

            await repository.AddAsync(product);

            return product.ProductId;
            //return 1;
        }
    }
}
