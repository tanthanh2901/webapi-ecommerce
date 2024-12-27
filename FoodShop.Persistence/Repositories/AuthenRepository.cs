using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Feature.Account.Models;
using FoodShop.Domain.Entities;
using FoodShop.Persistence;
using FoodShop.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using FoodShop.Application.Feature.Account.Register;
using FoodShop.Application.Models.Mail;
using FoodShop.Application.Contract.Infrastructure;

namespace FoodShop.Infrastructure.Repositories
{
    public class AuthenRepository : BaseRepository<AppUser>, IAuthenRepository
    {

        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ILogger<AuthenRepository> logger;
        private readonly IEmailService emailService;
        public AuthenRepository(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<AuthenRepository> logger,
            FoodShopDbContext dbContext,
            IEmailService emailService)
            : base(dbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.emailService = emailService;
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

                //user.EmailConfirmed = true;
                //await userManager.UpdateAsync(user);

                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = $"https://localhost:7226/authentication/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
                
                await emailService.SendEmail(new Email()
                {
                    To = registerModel.Email,
                    Subject = "Confirm your email",
                    Body = $"Please confirm your email by clicking <a href=\"{confirmationLink}\">here</a>."
                });

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
