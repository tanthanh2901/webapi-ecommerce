﻿using FoodShop.Application.Feature.Account.Models;
using FoodShop.Application.Feature.Account.Register;
using FoodShop.Domain.Entities;

namespace FoodShop.Application.Contract.Persistence
{
    public interface IAuthenRepository : IRepository<AppUser>
    {
        Task Logout();
        Task<AppUser> Login(LoginModel loginModel, CancellationToken cancellationToken = default);
        Task<RegisterResponse> Register(RegisterModel registerModel, CancellationToken cancellationToken = default);
    }
}
