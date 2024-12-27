using FoodShop.Application.Dto;
using FoodShop.Application.Feature.Account.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Feature.Account.Login
{
    public record LoginCommand(LoginModel LoginModel) : IRequest<TokenDto>;

}
