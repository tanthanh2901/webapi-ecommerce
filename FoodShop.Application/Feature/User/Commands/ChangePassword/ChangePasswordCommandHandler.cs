using AutoMapper;
using FoodShop.Application.Contract.Persistence;
using MediatR;

namespace FoodShop.Application.Feature.User.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IUserRepository userRepository;

        public ChangePasswordCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            await userRepository.ChangePassword(request.UserId, request.Model);
            return;
        }
    }
}
