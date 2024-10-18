using FoodShop.Application.Dto;
using MediatR;

namespace FoodShop.Application.Feature.User.Queries.GetUserInfo
{
    public class GetUserInfoQuery : IRequest<UserDto>
    {
        public int UserId;
    }
}
