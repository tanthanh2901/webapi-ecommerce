using FoodShop.Application.Contract.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Feature.User.Commands.UpdateUserInfo
{
    public class UpdateUserInfoCommandHandler : IRequestHandler<UpdateUserInfoCommand>
    {
        private readonly IUserRepository userRepository;

        public UpdateUserInfoCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task Handle(UpdateUserInfoCommand request, CancellationToken cancellationToken)
        {
            await userRepository.UpdateUserInfo(request.UserId, request.UpdateUserInfoViewModel);
            return;
        }
    }
}
