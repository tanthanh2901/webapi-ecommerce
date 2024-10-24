using FoodShop.API.Services;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Domain.Entities;
using FoodShop.Application.Feature.Categories.Commands.CreateCategory;
using FoodShop.Application.Feature.Categories.Commands.DeleteCategory;
using FoodShop.Application.Feature.Categories.Commands.UpdateCategory;
using FoodShop.Application.Feature.Categories.Queries.GetAllCategories;
using FoodShop.Application.Feature.Categories.Queries.GetProductDetails;
using FoodShop.Application.Feature.Products.Commands.CreateProduct;
using FoodShop.Application.Feature.Products.Commands.DeleteProduct;
using FoodShop.Application.Feature.Products.Commands.UpdateProduct;
using FoodShop.Application.Feature.Products.Queries.GetAllProducts;
using FoodShop.Application.Feature.Products.Queries.GetProductDetails;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FoodShop.Persistence.Repositories;
using FoodShop.Domain.Enum;
using Microsoft.AspNetCore.Identity;
using FoodShop.Application.Feature.User.Model;

namespace FoodShop.API.Controllers
{
    [Route("admin")]
    [ApiController]

    public class AdminController : Controller
    {
        private readonly RoleServices _roleService;
        private readonly IMediator mediatR;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IAdminRepository adminRepository;


        public AdminController(RoleServices roleService, IMediator mediatR, IProductRepository productRepository, IOrderRepository orderRepository, IAdminRepository adminRepository)
        {
            _roleService = roleService;
            this.mediatR = mediatR;
            _productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.adminRepository = adminRepository;
        }

        [HttpGet]
        [Route("roles/")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpPost]
        [Route("roles/create")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            var result = await _roleService.CreateRoleAsync(roleName);
            if (result.Succeeded)
            {
                return Ok(new { Message = $"Role '{roleName}' created successfully." });
            }

            return BadRequest(result.Errors);
        }

        [HttpPut]
        [Route("roles/update")]
        public async Task<IActionResult> UpdateRole(string roleId, [FromBody] string newRoleName)
        {
            var result = await _roleService.UpdateRoleAsync(roleId, newRoleName);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Role updated successfully." });
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete]
        [Route("roles/delete")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var result = await _roleService.DeleteRoleAsync(roleId);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Role deleted successfully." });
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("products")]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            var allProducts = await mediatR.Send(new GetAllProductQuery());
            return Ok(allProducts);
        }

        [HttpGet("products/{productId}")]
        public async Task<ActionResult<Product>> GetProductDetails(int productId)
        {
            var product = await mediatR.Send(new GetProductDetailsQuery() { ProductId = productId });
            return Ok(product);
        }

        [HttpPost("products/create")]
        public async Task<ActionResult<int>> Create(CreateProductCommand createrProductCommand)
        {
            var id = await mediatR.Send(createrProductCommand);
            return Ok(id);
        }

        [HttpPut("products/update")]
        public async Task<ActionResult<int>> Update(UpdateProductCommand updateProductCommand)
        {
            await mediatR.Send(updateProductCommand);
            return NoContent();
        }

        [HttpDelete("products/delete")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleteEventCommand = new DeleteProductCommand() { ProductId = id };
            await mediatR.Send(deleteEventCommand);
            return NoContent();
        }
        [HttpGet("products/search")]
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

        [HttpGet("categories")]
        public async Task<ActionResult<List<Category>>> GetAllCategories()
        {
            var listCategories = await mediatR.Send(new GetAllCategoriesQuery());

            return Ok(listCategories);
        }


        [HttpGet("categories/{categoryId}")]
        public async Task<ActionResult<Category>> GetCategoryDetails(int categoryId)
        {
            var category = await mediatR.Send(new GetCategoryDetailsQuery() { CategoryId = categoryId });
            return Ok(category);
        }

        [HttpPost("categories/create")]
        public async Task<ActionResult<int>> Create(CreateCategoryCommand createCategoryCommand)
        {
            var id = await mediatR.Send(createCategoryCommand);
            return Ok(id);
        }

        [HttpPut("categories/update")]
        public async Task<ActionResult<int>> Update(UpdateCategoryCommand updateCategoryCommand)
        {
            await mediatR.Send(updateCategoryCommand);
            return NoContent();
        }

        [HttpDelete("categories/delete")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var deleteCategoryCommand = new DeleteCategoryCommand() { CategoryId = id };
            await mediatR.Send(deleteCategoryCommand);
            return NoContent();
        }

        [HttpPost("orders/update")]
        public async Task<ActionResult<bool>> UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var update = await orderRepository.UpdateOrderStatusAsync(orderId, status);
            return Ok(update);
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<AppUser>>> GetAllUsers()
        {
            var users = await adminRepository.GetAllUsers();
            if (users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }
            return Ok(users);
        }

        [HttpGet("users/{userId}")]
        public async Task<ActionResult<AppUser>> GetUserDetails(int userId)
        {
            var user = await adminRepository.GetUserInfo(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }

        [HttpDelete("users/delete/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var success = await adminRepository.DeleteUser(userId); // Assuming _userService contains DeleteUser method.

            if (success)
            {
                return Ok(new { Message = "User deleted successfully." });
            }

            return BadRequest(new { Message = "Failed to delete user." });
        }


        [HttpPut("users/update/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserInfoViewModel model)
        {
            var success = await adminRepository.UpdateUserInfo(userId, model);

            if (success)
            {
                return Ok(new { Message = "User updated successfully." });
            }

            return BadRequest(new { Message = "Failed to update user." });
        }

        [HttpPost("users/{userId}/roles/assign")]
        public async Task<IActionResult> AssignRoleToUser(int userId, [FromBody] string role)
        {
            var success = await adminRepository.AssignRoleToUser(userId, role); // Assuming _userService is injected and contains AssignRoleToUser method.

            if (success)
            {
                return Ok(new { Message = $"Role '{role}' assigned to user successfully." });
            }

            return BadRequest(new { Message = "Failed to assign role to user." });
        }


    }
}
