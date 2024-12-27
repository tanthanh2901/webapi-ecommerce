using FoodShop.Application.Contract.Infrastructure;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Feature.Account.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly IAuthenRepository userRepository;

        public RegisterCommandHandler(IAuthenRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await userRepository.Register(request.RegisterModel);
        }
    }
}
