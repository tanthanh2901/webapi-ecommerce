using AutoMapper.Execution;
using FoodShop.Application.Dto;
using FoodShop.Application.Feature.Account.Login;
using FoodShop.Application.Feature.Account.Models;
using FoodShop.Application.Feature.Account.Register;
using FoodShop.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Contract.Persistence
{   
    public interface IAuthenRepository : IRepository<AppUser>
    {
        Task Logout();
        TokenDto Generate(AppUser user);
        Task<AppUser> Login(LoginModel loginModel, CancellationToken cancellationToken = default);
        Task<RegisterResponse> Register(RegisterModel registerModel, CancellationToken cancellationToken = default);
    }
}
