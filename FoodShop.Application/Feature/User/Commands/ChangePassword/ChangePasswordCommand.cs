using FoodShop.Application.Feature.User.Model;
using MediatR;

namespace FoodShop.Application.Feature.User.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest
    {
        public int UserId;
        public ChangePasswordViewModel Model;
    }
}
