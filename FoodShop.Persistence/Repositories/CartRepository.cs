using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Entities;
using FoodShop.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace FoodShop.Persistence.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly FoodShopDbContext dbContext;
        private readonly IProductRepository productRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(FoodShopDbContext dbContext, IProductRepository productRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = dbContext;
            this.productRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;

        }

        public int GetUserId()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var userID = int.Parse(user.Claims.First(c => c.Type == "userId").Value);
            return userID;
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await dbContext.Carts
                 .Include(c => c.Items)
                 .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                dbContext.CartItems.RemoveRange(cart.Items); // Remove all items
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<CartItem>> GetCartItems(int userId)
        {
            //var userID = GetUserId();

            //var cart = await dbContext.Carts
            //     .Include(c => c.Items)
            //     .FirstOrDefaultAsync(c => c.UserId == userId);

            //return (List<CartItem>)(cart?.Items ?? new List<CartItem>());

            var cart = await dbContext.Carts
                 .Include(c => c.Items)
                 .ThenInclude(ci => ci.Product) // Include Product in the CartItem
                 .FirstOrDefaultAsync(c => c.UserId == userId);

            return cart?.Items.ToList() ?? new List<CartItem>();
        }

        public async Task AddToCart(int userId, int productId, int quantity)
        {
            //var userID = GetUserId();

            var cart = await dbContext.Carts
                 .Include(c => c.Items)
                 .FirstOrDefaultAsync(c => c.UserId == userId);

            var product = await productRepository.GetByIdAsync(productId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                dbContext.Carts.Add(cart);
            }

            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);

            //var total = cart.Items.Select(c => c.Product.Price * c.Quantity).Sum();
            if (cartItem == null)
            {
                cartItem = new CartItem { ProductId = productId, Quantity = quantity, Product = product };
                cart.Items.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity; // Increase quantity
            }

            dbContext.Entry(product).State = EntityState.Unchanged;
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateCartItem(int userId, int productId, int quantity)
        {
            var cart = await dbContext.Carts
                       .Include(c => c.Items)
                       .FirstOrDefaultAsync(c => c.UserId == userId);

            var cartItem = cart?.Items.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCart(int userId, int productId)
        {
            var cart = await dbContext.Carts
                        .Include(c => c.Items)
                        .FirstOrDefaultAsync(c => c.UserId == userId);

            var cartItem = cart?.Items.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem != null)
            {
                cart.Items.Remove(cartItem);
                await   dbContext.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetShoppingCartTotal(int userId)
        {
            var cart = await dbContext.Carts
                  .Include(c => c.Items)
                  .FirstOrDefaultAsync(c => c.UserId == userId);

            var total = cart.Items.Select(c => c.Product.Price * c.Quantity).Sum();
            return total;
        }

        public async Task CreateOrderAsync(int userID, List<CartItem> cartProducts)
        {
            var order = new Order
            {
                UserId = userID,
                OrderDate = DateTime.UtcNow,
                ShipName = "John Doe",
                ShipAddress = "123 Main St",
                TotalAmount = cartProducts.Sum(p => p.Quantity * p.Product.Price),
                //Status = "Pending",
                OrderDetail = cartProducts.Select(p => new OrderDetail
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    UnitPrice = p.Product.Price
                }).ToList()
            };

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
        }

    }
}
