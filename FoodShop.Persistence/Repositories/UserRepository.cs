using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Dto;
using FoodShop.Application.Feature.User.Model;
using FoodShop.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodShop.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<UserRepository> _logger;
        private readonly FoodShopDbContext dbContext;

        public UserRepository(UserManager<AppUser> userManager, ILogger<UserRepository> logger, FoodShopDbContext dbContext)
        {
            this.userManager = userManager;
            _logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<AppUser> GetUser(int userId)
        {
            AppUser user = await dbContext.AppUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return user;
        }
        public async Task<UserDto> GetUserInfo(int userId)
        {
            var appUser =  await GetUser(userId);
            var roles = await userManager.GetRolesAsync(appUser);
            if (appUser == null)
            {
                throw new Exception("User not found");
            }

            return new UserDto
            {
                UserId = userId,
                UserName = appUser.UserName,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email,
                ShipName = appUser.ShipName,
                ShipAddress = appUser.ShipAddress,
                PhoneNumber = appUser.PhoneNumber,
                Roles = roles.ToList()
            };

        }

        public async Task UpdateUserInfo(int userId, UpdateUserInfoViewModel model)
        {
            var appUser = await GetUser(userId);

            if (appUser == null)
            {
                throw new Exception("User not found");
            }
            else
            {
                appUser.FirstName = model.FirstName;
                appUser.LastName = model.LastName;  
                appUser.ShipName = model.ShipName;
                appUser.ShipAddress = model.ShipAddress;
                appUser.PhoneNumber = model.PhoneNumber;
            }

            var result = await userManager.UpdateAsync(appUser);
            if (result.Succeeded)
            {
                // Optionally log success or perform any post-update actions
                _logger.LogInformation($"User information updated successfully for userId: {appUser.Id}");
            }
            else
            {
                // Handle the case where the update fails and log the errors
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError($"Failed to update user information for userId: {appUser.Id}. Errors: {errors}");

                // Optionally throw an exception or return a custom error response
                throw new Exception($"Failed to update user information. Errors: {errors}");
            }
        }

        public async Task ChangePassword(int userId, ChangePasswordViewModel model)
        {
            var appUser = await GetUser(userId);

            if (appUser == null)
            {
                throw new Exception("User not found");
            }

            var result = await userManager.ChangePasswordAsync(appUser, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                // You can log the successful password change
                _logger.LogInformation($"Password changed successfully for user: {appUser.UserName}");

                // Optionally, you could send a confirmation email to the user
                // await emailService.SendPasswordChangedConfirmationEmail(appUser.Email);

                return; // Password change was successful
            }

            // Handle failure
            else
            {
                // Collect error messages
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                // Log the error (optional)
                _logger.LogError($"Failed to change password for user: {appUser.UserName}. Errors: {errors}");

                // Throw an exception with the error details
                throw new Exception($"Failed to change password: {errors}");
            }
        }

    }
}
