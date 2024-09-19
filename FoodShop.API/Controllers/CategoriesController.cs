using FoodShop.Application.Entities;
using FoodShop.Application.Feature.Categories.Commands.CreateCategory;
using FoodShop.Application.Feature.Categories.Commands.DeleteCategory;
using FoodShop.Application.Feature.Categories.Commands.UpdateCategory;
using FoodShop.Application.Feature.Categories.Queries.GetAllCategories;
using FoodShop.Application.Feature.Categories.Queries.GetProductDetails;
using FoodShop.Application.Feature.Products.Commands.CreateProduct;
using FoodShop.Application.Feature.Products.Commands.DeleteProduct;
using FoodShop.Application.Feature.Products.Commands.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FoodShop.API.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoriesController : Controller
    {
        private readonly IMediator mediatR;

        public CategoriesController(IMediator mediatR)
        {
            this.mediatR = mediatR;
        }

        [HttpGet(Name ="GetAllCategories")]
        public async Task<ActionResult<List<Category>>> GetAllCategories()
        {
            var listCategories = await mediatR.Send(new GetAllCategoriesQuery());

            return Ok(listCategories);
        }


        [HttpGet("{categoryId}")]
        public async Task<ActionResult<Category>> GetCategoryDetails(int categoryId)
        {
            var category = await mediatR.Send(new GetCategoryDetailsQuery() { CategoryId = categoryId });
            return Ok(category);
        }

    }
}
