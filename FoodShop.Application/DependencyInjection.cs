using FoodShop.Application.Feature.Notification;
using FoodShop.Application.Feature.Payment.VnPay;
using FoodShop.Application.Services;
using FoodShop.Application.Services.Payment;
using FoodShop.Application.Services.Payment.ZaloPay;
using Microsoft.Extensions.DependencyInjection;

namespace FoodShop.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies();
            services.AddAutoMapper(assembly);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembly));
            services.AddScoped<RoleServices>();

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IVnPayService>(provider => provider.GetService<IPaymentService>());
            services.AddScoped<IZaloPayService>(provider => provider.GetService<IPaymentService>());

            services.AddHttpClient<CurrencyExchangeService>(client =>
            {
                client.BaseAddress = new Uri("https://api.exchangeratesapi.io/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddSignalR();
            services.AddScoped<NotificationService>();
            services.AddHttpContextAccessor();
            return services;
        }
    }
}
