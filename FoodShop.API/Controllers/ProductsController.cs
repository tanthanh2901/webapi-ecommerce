using FoodShop.Application.Entities;
using FoodShop.Application.Feature.Products.Queries.GetAllProducts;
using FoodShop.Application.Feature.Products.Queries.GetProductDetails;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FoodShop.Application.Contract.Persistence;

namespace FoodShop.API.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : Controller
    {
        private readonly IMediator mediatR;
        private readonly IProductRepository _productRepository;

        public ProductsController(IMediator mediatR, IProductRepository productRepository)
        {
            this.mediatR = mediatR;
            _productRepository = productRepository;
        }

        [HttpGet(Name ="GetAllProducts")]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            var allProducts = await mediatR.Send(new GetAllProductQuery());
            return Ok(allProducts);
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<Product>> GetProductDetails(int productId)
        {
            var product = await mediatR.Send(new GetProductDetailsQuery() { ProductId = productId});
            return Ok(product);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IReadOnlyList<Product>>> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            var products = await _productRepository.SearchProduct(query);

            if (products == null || products.Count == 0)
            {
                return NotFound("No products found matching the search criteria.");
            }

            return Ok(products);
        }
    }
}
