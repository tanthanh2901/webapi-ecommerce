using AutoMapper;
using FoodShop.Application.Contract.Infrastructure;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Dto;
using FoodShop.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Feature.Account.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenDto>
    {
        private readonly IAuthenRepository userRepository;
        private readonly IMapper mapper;

        public LoginCommandHandler(IAuthenRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<TokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.Login(request.LoginModel, cancellationToken);

            var token = userRepository.Generate(user);

            return token;
            //return new LoginResponse
            //{
            //    AccessToken = token.accessToken,
            //    RefreshToken = token.refreshToken,
            //    Username = user.UserName,
            //    UserId = user.Id
            //};

        }
    }
}
