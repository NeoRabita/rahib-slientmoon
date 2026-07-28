using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Account;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Messaging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Profile.Queries.GetMe
{
    public class GetMeQuery : IQuery<UserDto>, IRequireAuth
    {
    }

    public class GetMeQueryHandler : IQueryHandler<GetMeQuery, UserDto>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetMeQueryHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public GetMeQueryHandler(IUow uow, IAppLogger<GetMeQueryHandler> logger, ICurrentUserService currentUserService)
        {
            _uow = uow;
            _logger = logger;
            _currentUserService = currentUserService;
        }


        public async Task<Result<UserDto>> Handle(GetMeQuery query, CancellationToken ct)
        {
            string userId = _currentUserService.UserId;

            _logger.LogInformation("GetMe started. UserId: {UserId}", userId);

            var user = await _uow.UserRepository.GetByIdAsync(userId);

            _logger.LogInformation("GetMe done. UserId: {UserId}", userId);

            return new UserDto
            {
                Id = userId,
                Name = user.Name,
                Email = user.Email,
                EmailVerified = user.EmailVerified,
                AvatarUrl = user.AvatarUrl ?? string.Empty,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
