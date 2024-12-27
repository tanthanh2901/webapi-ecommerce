using AutoMapper;
using FoodShop.Application.Contract.Infrastructure;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Domain.Entities;
using MediatR;

namespace FoodShop.Application.Feature.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IRepository<Product> repository;
        private readonly IMapper mapper;
        private readonly IS3Service s3Service;

        public CreateProductCommandHandler(IRepository<Product> repository, IMapper mapper, IS3Service s3Service)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.s3Service = s3Service;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var imgUrl = await s3Service.UploadFileAsync(request.Image.OpenReadStream());

            var product = mapper.Map<Product>(request);
            product.ImageUrl = imgUrl;

            await repository.AddAsync(product);

            return product.ProductId;
            //return 1;
        }
    }
}
