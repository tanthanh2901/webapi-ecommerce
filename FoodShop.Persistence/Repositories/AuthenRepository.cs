using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Feature.Account.Models;
using FoodShop.Domain.Entities;
using FoodShop.Persistence;
using FoodShop.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using FoodShop.Application.Feature.Account.Register;
using FoodShop.Application.Models.Mail;

namespace FoodShop.Infrastructure.Repositories
{
    public class AuthenRepository : BaseRepository<AppUser>, IAuthenRepository
    {

        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ILogger<AuthenRepository> logger;

        public AuthenRepository(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<AuthenRepository> logger,
            FoodShopDbContext dbContext)
            : base(dbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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
            var user = new AppUser
            {
                UserName = registerModel.Email,
                Email = registerModel.Email 
            };

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
    }
}
