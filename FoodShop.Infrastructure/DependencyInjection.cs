using Amazon;
using Amazon.S3;
using FoodShop.Application.Contract.Infrastructure;
using FoodShop.Infrastructure.Authentication;
using FoodShop.Infrastructure.AWS;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FoodShop.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services
            , IConfiguration configuration)
        {
            services.AddScoped<IJwtProvider, JwtProvider>();

            services.Configure<S3Settings>(configuration.GetSection("S3Settings"));
            services.AddSingleton<IAmazonS3>(sp =>
            {
                var s3Settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(s3Settings.Region)
                };

                return new AmazonS3Client(s3Settings.AccessKey, s3Settings.SecretKey, config);
            });
            services.AddScoped<IS3Service, S3Service>();
            
            return services;
        }
    }
}
