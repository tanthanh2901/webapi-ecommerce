using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Feature.Account.Login;
using FoodShop.Application.Feature.Account.Models;
using FoodShop.Application.Feature.Account.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodShop.API.Services;
using Microsoft.AspNetCore.Identity;
using FoodShop.Domain.Entities;
using FoodShop.Application.Contract.Infrastructure;
using FoodShop.Application.Dto;

namespace FoodShop.API.Controllers
{
    [Route("authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator mediatR;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthenRepository authenRepository;
        private readonly AuthenticationServices authenticationService;

        public AuthenticationController(IMediator mediatR, IAuthenRepository authenRepository, AuthenticationServices authenticationService, UserManager<AppUser> userManager)
        {
            this.mediatR = mediatR;
            this.authenRepository = authenRepository;
            this.authenticationService = authenticationService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var tokenDto = await mediatR.Send(new LoginCommand(loginModel));
                authenticationService.SetTokenCookie(tokenDto, HttpContext);

                var csrfToken = Guid.NewGuid().ToString(); // Generate a random token

                Response.Cookies.Append("XSRF-TOKEN", csrfToken, new CookieOptions
                {
                    HttpOnly = true,  
                    Secure = true,   
                    SameSite = SameSiteMode.Strict
                });

                return Ok(new { accessToken = tokenDto.accessToken });
              
            }

            return BadRequest("Invalid model state.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var loginResponse = await mediatR.Send(new RegisterCommand(registerModel));
                return Ok(loginResponse);

            }

            return BadRequest("Invalid model state.");
        }

        [HttpPost("logout")]
        [Authorize] // Ensure only authenticated users can logout
        public async Task<IActionResult> Logout()
        {
            // Sign out the user
            await authenRepository.Logout();

            // Clear the authentication cookie
            Response.Cookies.Delete("accessToken");
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenDto tokenDto)
        {
            var newTokens = await authenRepository.RefreshToken(tokenDto);

            // Set the new tokens in cookies
            authenticationService.SetTokenCookie(newTokens, HttpContext);

            return Ok(newTokens);
        }


    }




}
