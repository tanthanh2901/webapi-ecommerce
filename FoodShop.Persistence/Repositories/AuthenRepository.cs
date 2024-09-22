using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Feature.Account.Models;
using FoodShop.Domain.Entities;
using FoodShop.Persistence;
using FoodShop.Persistence.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Security.Policy;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FoodShop.Application.Feature.Account.Register;
using FoodShop.Application.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security;
using Microsoft.Extensions.Configuration;

namespace FoodShop.Infrastructure.Repositories
{
    public class AuthenRepository : BaseRepository<AppUser>, IAuthenRepository
    {
        //private readonly FoodShopDbContext dbContext;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ILogger<AuthenRepository> logger;
        private readonly IConfiguration _configuration; 


        //private readonly IUserStore<IdentityUser> _userStore;
        //private readonly IUserEmailStore<IdentityUser> _emailStore;

        public AuthenRepository(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<AuthenRepository> logger,
            //IUserStore<IdentityUser> userStore,
            //IUserEmailStore<IdentityUser> emailStore,
            FoodShopDbContext dbContext,
            IConfiguration configuration
           ) : base(dbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _configuration = configuration;
            //_userStore = userStore;
            //_emailStore = emailStore;
        }

        private FoodShopDbContext DbContext { get; }

        public async Task<AppUser> Login(LoginModel model, CancellationToken cancellationToken = default)
        {
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            AppUser user = null;
            //AppUser user = new AppUser();
            if (result.Succeeded)
            {
                logger.LogInformation("login success");
                user = await userManager.FindByEmailAsync(model.Email);
            }
            else if (result.RequiresTwoFactor)
            {
                // Handle two-factor authentication case
            }
            else if(result.IsLockedOut)
            {
                // Handle lockout scenario
            }
            else
            {
                logger.LogError("login fail");
            }
            return user;
        }


        public async Task<RegisterResponse> Register(RegisterModel registerModel, CancellationToken cancellationToken = default)
        {
            //var user = CreateUser();
            var user = new AppUser
            {
                UserName = registerModel.Email,
                Email = registerModel.Email 
            };
            //await _userStore.SetUserNameAsync(user, registerModel.Email, CancellationToken.None);
            //await _emailStore.SetEmailAsync(user, registerModel.Email, CancellationToken.None);

            var result = await userManager.CreateAsync(user, registerModel.Password);

            var registerResponse = new RegisterResponse();

            if (result.Succeeded)
            {
                logger.LogInformation("User created a new account with password.");

                registerResponse.Username = user.UserName;               
                registerResponse.UserId = user.Id;               
                registerResponse.Message = "User created";

                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);

                await signInManager.SignInAsync(user, isPersistent: false);
            }
            foreach (var error in result.Errors)
            {
                logger.LogError(error.ToString());
            }

            return registerResponse;
        }

        public async Task Logout()
        {
            await signInManager.SignOutAsync();
            logger.LogInformation("User log out.");

        }

        public TokenDto Generate(AppUser user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Jwt:SecretForKey"]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),

                new Claim("name", user.UserName), // User's full name
                //new Claim("role", user), // User's role
                new Claim("userId", user.Id.ToString()), // User ID
                //new Claim("cartId", user.Cart?.CartId.ToString() ?? string.Empty)
            };

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var accessToken = new JwtSecurityTokenHandler()
               .WriteToken(jwtSecurityToken);

            var refreshToken = GenerateRefreshToken();

            var tokenDto = new TokenDto(accessToken, refreshToken);

            return tokenDto;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private ClaimsPrincipal GetClaimsPrincipalFromExpireToken(string token)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretForKey"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityException("Invalid token");
            }

            return principal;
        }
        

    }
}
