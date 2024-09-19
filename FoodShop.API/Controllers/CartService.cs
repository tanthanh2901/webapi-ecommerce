using FoodShop.Domain.Entities;
using Newtonsoft.Json;
using SendGrid.Helpers.Errors.Model;
using System.Text;

public class CartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private string EncodeCookie(string value)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }

    private string DecodeCookie(string encodedValue)
    {
        //return Encoding.UTF8.GetString(Convert.FromBase64String(encodedValue));
        try
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedValue));
        }
        catch (FormatException)
        {
            // Log the error
            Console.WriteLine($"Invalid Base64 string: {encodedValue}");
            // Return an empty JSON array as a fallback
            return "[]";
        }
    }

    public List<CartProduct> GetCartFromCookie()
    {
        var request = _httpContextAccessor.HttpContext.Request;
        var cartCookie = request.Cookies["cart"];
        if (cartCookie != null)
        {
            var cartJson = DecodeCookie(cartCookie);
            return JsonConvert.DeserializeObject<List<CartProduct>>(cartJson);
        }

        return new List<CartProduct>();
    }

    public void AddProductToCartCookie(CartProduct newProduct)
    {
        var cartProducts = GetCartFromCookie();
        var existingProduct = cartProducts.FirstOrDefault(p => p.ProductID == newProduct.ProductID);

        if (existingProduct != null)
        {
            existingProduct.Quantity += newProduct.Quantity;
        }
        else
        {
            cartProducts.Add(newProduct);
        }

        var updatedCartJson = JsonConvert.SerializeObject(cartProducts);

        // Update cookie
        CreateCookie(updatedCartJson);

    }

    public void ClearCart()
    {
        var response = _httpContextAccessor.HttpContext.Response;
        response.Cookies.Delete("cart");
    }

    // get number of products
    public int GetNumberOfProducts(string encodedCartJson)
    {
        int numberOfCartItems = GetListProducts(encodedCartJson).Count;
        return numberOfCartItems;
    }

    // get list of product in cart
    public List<CartProduct> GetListProducts(string encodedCartJson)
    {
        if (encodedCartJson != null)
        {
            string cartJson = DecodeCookie(encodedCartJson);

            // Deserialize the JSON to a list of CartProduct
            var listProducts = JsonConvert.DeserializeObject<List<CartProduct>>(cartJson);

            return listProducts;
        }

        return new List<CartProduct>();
    }

    public CartOrderInfo UpdateCartCookie(string encodedCartJson, List<CartProduct> products)
    {
        if (products == null || !products.Any())
        {
            throw new NotFoundException("Product does not found");
        }

        // Decode and deserialize the existing cart
        var items = new List<CartProduct>();
        if (!string.IsNullOrEmpty(encodedCartJson))
        {
            var cartJson = DecodeCookie(encodedCartJson);
            items = JsonConvert.DeserializeObject<List<CartProduct>>(cartJson);
        }

        // Fetch products
        var productIDs = products.Select(p => p.ProductID).ToList();
        var fetchedProducts = products.Where(product => productIDs.Contains(product.ProductID)).ToList();

        foreach (var currentProduct in fetchedProducts)
        {
            var cartProduct = products.FirstOrDefault(p => p.ProductID == currentProduct.ProductID)
                ?? throw new NotFoundException($"Product {currentProduct.ProductID} does not found");

            // Check whether the product exists in cart or not
            var existingProduct = items.FirstOrDefault(item => item.ProductID == cartProduct.ProductID);
            if (existingProduct != null)
            {
                existingProduct.Quantity = cartProduct.Quantity;
            }
            else
            {
                items.Add(cartProduct);
            }
        }

        // Serialize the updated cart and update the cookie
        var updatedCartJson = JsonConvert.SerializeObject(items);
        CreateCookie(updatedCartJson);

        return CreateCartOrder(items);
    }

    private void CreateCookie(string value)
    {
        string cookie = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        _httpContextAccessor.HttpContext.Response.Cookies.Append("cart", cookie, new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(7),
            HttpOnly = true,
            Secure = true,
            Path = "/"
        }); ;
    }

    private CartOrderInfo CreateCartOrder(List<CartProduct>? products)
    {
        if (products == null || products.Count == 0)
        {
            return new CartOrderInfo();
        }

        // Extract productIDs from products
        var productIDs = products.Select(p => p.ProductID).ToList();
        var fetchedProducts = products.Where(product => productIDs.Contains(product.ProductID)).ToList();

        var order = new CartOrderInfo();

        foreach (var product in products)
        {
            // Find the product


            // Add product to order

            // Add product
            //order.products.Add(item);

            //// Calculate total price of order
            //if (product.IsSelected)
            //{
            //    order.TotalPrice = bigDecimalService.Formatter(order.TotalPrice.Add(extendedPrice));
            //}
        }

        return order;
    }

    public CartOrderInfo RemoveProduct(int? productId, string encodedCartJson, HttpResponse response)
    {
        if (productId == null)
        {
            throw new ArgumentNullException(nameof(productId), "Product ID cannot be null.");
        }

        var items = new List<CartProduct>();

        if (!string.IsNullOrEmpty(encodedCartJson))
        {
            try
            {
                // Attempt to decode the Base64 string
                var cartJson = DecodeCookie(encodedCartJson);
                items = JsonConvert.DeserializeObject<List<CartProduct>>(cartJson);
            }
            catch (FormatException ex)
            {
                // Handle invalid Base64 string
                throw new InvalidOperationException("Invalid cart JSON format. Cannot decode Base64 string.", ex);
            }

            // Filter out the product by ID
            var orderItems = items.Where(item => item.ProductID != productId).ToList();

            // Convert the list back to JSON
            var updatedCartJson = JsonConvert.SerializeObject(orderItems);

            // Update the cookie with the new cart data
            CreateCookie(updatedCartJson);

            return CreateCartOrder(orderItems);
        }

        return CreateCartOrder(items);
    }

    private CartOrderInfo GetCartOrder(string encodedCartJson)
    {
        if (string.IsNullOrEmpty(encodedCartJson))
        {
            return new CartOrderInfo();
        }

        var cartJson = DecodeCookie(encodedCartJson);
        var listProducts = JsonConvert.DeserializeObject<List<CartProduct>>(cartJson);

        return CreateCartOrder(listProducts);
    }
}
