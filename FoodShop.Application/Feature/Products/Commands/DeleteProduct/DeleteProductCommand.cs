using MediatR;

namespace FoodShop.Application.Feature.Products.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest
    {
        public int ProductId;
    }
}
