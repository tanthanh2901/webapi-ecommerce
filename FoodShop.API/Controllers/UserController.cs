using FoodShop.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodShop.API.Controllers
{
    [Route("user")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("userInformation")]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Create a ViewModel to return the user info
            var userInfo = new AppUser
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            return Ok(userInfo);
        }

        [HttpPost("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo(UpdateUserInfoViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }


            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok("User info updated successfully");
            }

            // Handle errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        public class UpdateUserInfoViewModel
        {
        }

        public class ChangePasswordViewModel
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password changed successfully");
            }

            // Handle errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }
    }
}
