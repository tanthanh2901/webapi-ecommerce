using FoodShop.Application.Contract.Infrastructure;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Dto;
using MediatR;

namespace FoodShop.Application.Feature.Account.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenDto>
    {
        private readonly IAuthenRepository authenRepository;
        private readonly IJwtProvider jwtProvider;

        public LoginCommandHandler(IAuthenRepository userRepository, IJwtProvider jwtProvider)
        {
            this.authenRepository = userRepository;
            this.jwtProvider = jwtProvider;
        }

        public async Task<TokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await authenRepository.Login(request.LoginModel, cancellationToken);

            var token = await jwtProvider.Generate(user);
            return token;            
        }
    }
}
