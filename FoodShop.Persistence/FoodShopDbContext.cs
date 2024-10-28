using FoodShop.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Persistence
{
    public class FoodShopDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public FoodShopDbContext(DbContextOptions<FoodShopDbContext> options)
               : base(options)
        {
        }

        public FoodShopDbContext(DbSet<AppUser> appUsers, DbSet<Cart> carts, DbSet<CartItem> cartItems, DbSet<Category> categories, DbSet<Order> orders, DbSet<OrderDetail> orderDetails, DbSet<Product> products, DbSet<Payment> payment, DbSet<PaymentMethod> paymentMethod)
        {
            AppUsers = appUsers;
            Carts = carts;
            CartItems = cartItems;
            Categories = categories;
            Orders = orders;
            OrderDetails = orderDetails;
            Products = products;
            Payment = payment;
            PaymentMethod = paymentMethod;
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<PaymentMethod> PaymentMethod { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)") // Specify the SQL type directly
                    .HasPrecision(18, 2); // Or specify precision and scale separately
            });

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetail)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId);

            modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetail)
            .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<Cart>()
               .HasMany(c => c.Items)
               .WithOne(ci => ci.Cart)
               .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);

        }
    }
}