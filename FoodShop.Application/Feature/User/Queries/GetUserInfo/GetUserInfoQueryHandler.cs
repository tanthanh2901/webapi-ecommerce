using AutoMapper;
using FoodShop.Application.Contract.Persistence;
using FoodShop.Application.Dto;
using MediatR;

namespace FoodShop.Application.Feature.User.Queries.GetUserInfo
{
    public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, UserDto>
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public GetUserInfoQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
        {
            var appuser = await userRepository.GetUserInfo(request.UserId);
            var userDto = mapper.Map<UserDto>(appuser);
            return userDto;
        }
    }
}
