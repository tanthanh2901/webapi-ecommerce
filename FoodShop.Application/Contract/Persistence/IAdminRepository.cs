using FoodShop.Application.Feature.User.Model;
using FoodShop.Domain.Entities;

namespace FoodShop.Application.Contract.Persistence
{
    public interface IAdminRepository
    {
        Task<List<AppUser>> GetAllUsers();
        Task<AppUser> GetUserInfo(int userId);
        Task<bool> UpdateUserInfo(int userId, UpdateUserInfoViewModel model);
        Task<bool> DeleteUser(int userId);
        Task<bool> AssignRoleToUser(int userId, string role);

    }
}
