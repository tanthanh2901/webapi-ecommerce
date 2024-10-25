using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Dto;
using MediatR;

namespace FoodShop.Application.Feature.Account.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenDto>
    {
        private readonly IAuthenRepository authenRepository;

        public LoginCommandHandler(IAuthenRepository userRepository)
        {
            this.authenRepository = userRepository;
        }

        public async Task<TokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await authenRepository.Login(request.LoginModel, cancellationToken);

            var token = await authenRepository.Generate(user);
            return token;            
        }
    }
}
