using FoodShop.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FoodShop.Application.Services
{
    public class RoleServices
    {
        private RoleManager<AppRole> _roleManager;
        private UserManager<AppUser> _userManager;

        public RoleServices(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            this._roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<List<AppRole>> GetAllRolesAsync()
        {
            return await Task.FromResult(_roleManager.Roles.ToList());
        }

        public async Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            if(string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Role name can not be null or empty.");
            }

            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if(roleExist)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' already exist" });
            }
            return await _roleManager.CreateAsync(new AppRole(roleName));
        }

        public async Task<IdentityResult> UpdateRoleAsync(string roleId, string newRoleName)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role id '{roleId}' not found" });
            }
            role.Name = newRoleName;
            return await _roleManager.UpdateAsync(role);
        }

        public async Task<IdentityResult> DeleteRoleAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role id '{roleId}' not found" });
            }
            return await _roleManager.DeleteAsync(role);

        }

        public async Task<IdentityResult> AddUserToRoleAsync(AppUser user, string roleName)
        {
            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IList<string>> GetUserRolesAsync(AppUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> IsUserInRoleAsync(AppUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }


    }
}
