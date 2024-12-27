using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Feature.User.Model;
using FoodShop.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodShop.Persistence.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<UserRepository> _logger;


        public AdminRepository(UserManager<AppUser> userManager, ILogger<UserRepository> logger = null)
        {
            this.userManager = userManager;
            _logger = logger;
        }

        public async Task<bool> AssignRoleToUser(int userId, string role)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogInformation("User not found.");
                return false;
            }

            var result = await userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Role '{role}' assigned to user {user.UserName}.");
                return true;
            }
            else
            {
                _logger.LogError($"Failed to assign role '{role}' to user {user.UserName}.");
                return false;
            }
        }

        public async Task<bool> DeleteUser(int userId)
        {
            var user = await GetUserInfo(userId);
            if (user == null)
            {
                _logger.LogInformation("User not found.");
                return false;
            }

            // Perform deletion logic here
            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User deleted successfully.");
                return true;
            }
            else
            {
                _logger.LogError("Failed to delete user.");
                return false;
            }
        }


        public async Task<List<AppUser>> GetAllUsers()
        {
            var users = await userManager.Users.ToListAsync();        
            if(users == null || users.Count == 0)
            {
                _logger.LogInformation("No users found");
            }

            return users;
        }

        public async Task<AppUser> GetUserInfo(int userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogInformation("User not found");
            }

            return user;
        }

        public async Task<bool> UpdateUserInfo(int userId, UpdateUserInfoViewModel model)
        {
            var user = await GetUserInfo(userId);

            if (user == null)
            {
                _logger.LogError("User not found.");
                return false;
            }

            user.ShipName = model.ShipName;
            user.ShipAddress = model.ShipAddress;
            user.PhoneNumber = model.PhoneNumber;

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User updated successfully.");
                return true; // Update successful
            }
            else
            {
                _logger.LogError("Failed to update user.");
                return false; // Update failed
            }
        }

    }
}
