using FoodShop.Application.Dto;

namespace FoodShop.API.Services
{
    public class AuthenticationServices
    {
        public void SetTokenCookie(TokenDto tokenDto, HttpContext httpContext)
        {
            httpContext.Response.Cookies.Append("accessToken", tokenDto.accessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.Now.AddHours(1),
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Strict,
                });
            httpContext.Response.Cookies.Append("refreshToken", tokenDto.refreshToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(5),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None,
                });          
        }
    }
}
