using FoodShop.API.Services;
using FoodShop.Application;
using FoodShop.Domain.Entities;
using FoodShop.Infrastructure;
using FoodShop.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<FoodShopDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("GloboTicketManageConnectionString")));


builder.Services.AddIdentity<AppUser, AppRole>()
       .AddEntityFrameworkStores<FoodShopDbContext>()
       .AddDefaultTokenProviders();

builder.Services
    .AddApplicationServices()
    .AddPersistenceServices(builder.Configuration)
    .AddInfrastructureServices();

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "X-CSRF-TOKEN"; // Customize token name
    options.HeaderName = "X-CSRF-TOKEN";  // Set the header that will carry the token
});

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretForKey"]);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins("http://localhost:5173", "https://localhost:44493") // React app URL
            .AllowAnyMethod()                     // Allow all HTTP methods
            .AllowAnyHeader()                     // Allow any header
            .AllowCredentials());                 // Allow credentials if necessary
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
        //OnMessageReceived = ctx =>
        //{
        //    ctx.Request.Cookies.TryGetValue("accessToken", out var accessToken);
        //    if (!string.IsNullOrEmpty(accessToken))
        //    {
        //        ctx.Token = accessToken;
        //    }
        //    return Task.CompletedTask;
        //}
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("accessToken"))
            {
                context.Token = context.Request.Cookies["accessToken"];
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddDistributedMemoryCache(); // In-memory cache to store session data

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "cart_id"; // Custom name for the session cookie
    options.Cookie.HttpOnly = true; // Ensures the cookie is accessible only by the server
    options.Cookie.IsEssential = true; // Mark the session cookie as essential
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Require HTTPS
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<RoleServices>();
builder.Services.AddScoped<AuthenticationServices>();

builder.Services.AddControllers();
     //.AddJsonOptions(options =>
     //{
     //    //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
     //    //options.JsonSerializerOptions.WriteIndented = true;

     //    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
     //    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
     //});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

app.UseCors("AllowReactApp");
//app.UseCors("AllowReactApp");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllers();

app.Run();


