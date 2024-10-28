using FoodShop.Application.Dto;
using FoodShop.Application.Feature.User.Model;
using FoodShop.Domain.Entities;

namespace FoodShop.Application.Contract.Persistence
{
    public interface IUserRepository
    {
        Task<UserDto> GetUserInfo(int userId);
        Task UpdateUserInfo(int userId, UpdateUserInfoViewModel model);
        Task ChangePassword(int userId, ChangePasswordViewModel model);
    }
}
