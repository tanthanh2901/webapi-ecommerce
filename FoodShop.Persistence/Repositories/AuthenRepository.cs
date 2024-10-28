using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Feature.Account.Models;
using FoodShop.Domain.Entities;
using FoodShop.Persistence;
using FoodShop.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text;
using FoodShop.Application.Feature.Account.Register;
using FoodShop.Application.Dto;
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

        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ILogger<AuthenRepository> logger;
        private readonly IConfiguration _configuration; 

        public AuthenRepository(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<AuthenRepository> logger,
            FoodShopDbContext dbContext,
            IConfiguration configuration
           ) : base(dbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _configuration = configuration;
        }

        private FoodShopDbContext DbContext { get; }

        public async Task<AppUser> Login(LoginModel model, CancellationToken cancellationToken = default)
        {
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                //// Handle specific cases before logging an error
                //if (result.RequiresTwoFactor)
                //{
                //    // Handle two-factor authentication case, if necessary
                //}
                //else if (result.IsLockedOut)
                //{
                //    // Handle lockout scenario, if necessary
                //}
                //else
                //{
                //}
                logger.LogError("Login failed for user: {Email}", model.Email); 
                return null; // Early exit on failure
            }

            logger.LogInformation("Login successful for user: {Email}", model.Email); 
            var user = await userManager.FindByEmailAsync(model.Email);

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
        private ClaimsPrincipal GetClaimsPrincipalFromExpireToken(string token)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretForKey"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
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

        public async Task<TokenDto> Generate(AppUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Jwt:SecretForKey"]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.UserName.ToString()),
                new Claim("username", user.UserName.ToString()),

                new Claim("userId", user.Id.ToString()),
            };

            foreach (var role in roles)
            {
                claimsForToken.Add(new Claim(ClaimTypes.Role, role));  // or use "role" as the claim type
            }

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

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await userManager.UpdateAsync(user);

            return new TokenDto(accessToken, refreshToken);
        }

        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetClaimsPrincipalFromExpireToken(tokenDto.accessToken);

            foreach (var claim in principal.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            var userName = principal.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
            var user = await userManager.FindByNameAsync(userName);
            if (user == null || user.RefreshToken != tokenDto.refreshToken ||
                user.RefreshTokenExpiryTime < DateTime.Now)
            {
                throw new Exception("Invalid or expired refresh token.");
            }

            return await Generate(user);
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
    }
}
