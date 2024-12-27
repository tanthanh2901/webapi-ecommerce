using FoodShop.Application.Feature.Account.Models;
using FoodShop.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FoodShop.Application.Feature.Account.Register
{
    public record RegisterCommand( RegisterModel RegisterModel) : IRequest<RegisterResponse>;
    

}
