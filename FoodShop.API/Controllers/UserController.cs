﻿using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Feature.User.Commands.ChangePassword;
using FoodShop.Application.Feature.User.Commands.UpdateUserInfo;
using FoodShop.Application.Feature.User.Model;
using FoodShop.Application.Feature.User.Queries.GetUserInfo;
using FoodShop.Domain.Entities;
using MediatR;
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
        private readonly IMediator mediatR;
        private readonly IUserRepository userRepository;
        public UserController(UserManager<AppUser> userManager, IMediator mediatR, IUserRepository userRepository)
        {
            _userManager = userManager;
            this.mediatR = mediatR;
            this.userRepository = userRepository;
        }

        private async Task<int> GetUserId()
        {
            var user = await _userManager.GetUserAsync(User);
            return (int)(user?.Id);
        }

        [HttpGet("userInformation")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = await GetUserId();
            if (userId == null)
            {
                return NotFound("User not found");
            }

            //var userInfo = await mediatR.Send(new GetUserInfoQuery() { UserId = userId });
            var userInfo = await userRepository.GetUserInfo(userId);
            return Ok(userInfo);
        }

        [HttpPost("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo(UpdateUserInfoViewModel model)
        {
            var userId = await GetUserId();

            await mediatR.Send(new UpdateUserInfoCommand()
            {
                UserId = userId,
                UpdateUserInfoViewModel = model
            });

            return NoContent();
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userId = await GetUserId();

            await mediatR.Send(new ChangePasswordCommand() { UserId = userId, Model = model });
            return Ok();
            
        }
    }
}
