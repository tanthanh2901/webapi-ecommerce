using FoodShop.Application.Feature.User.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Feature.User.Commands.UpdateUserInfo
{
    public class UpdateUserInfoCommand : IRequest
    {
        public int UserId { get; set;}
        public UpdateUserInfoViewModel UpdateUserInfoViewModel { get; set;}
    }
}
