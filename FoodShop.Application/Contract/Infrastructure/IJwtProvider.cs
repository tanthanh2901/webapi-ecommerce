using AutoMapper.Execution;
using FoodShop.Application.Dto;
using FoodShop.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Contract.Infrastructure
{
    public interface IJwtProvider
    {
        Task<TokenDto> Generate(AppUser user);
        Task<TokenDto> RefreshToken(TokenDto tokenDto);
    }
}
