using AutoMapper;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Feature.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IRepository<Product> repository;
        private readonly IMapper mapper;

        public UpdateProductCommandHandler(IRepository<Product> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var prodcutToUpdate = await repository.GetByIdAsync(request.ProductId);
            mapper.Map(request, prodcutToUpdate, typeof(UpdateProductCommand), typeof(Product));

            await repository.UpdateAsync(prodcutToUpdate);
        }
    }
}
