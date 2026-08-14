using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Account;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Profile.Commands.UpdateProfile
{
    public class UpdateProfileCommand : ICommand<UserDto>
    {
        public string Name { get; }
        public string AvatarUrl { get; }

        public UpdateProfileCommand(string name, string avatarUrl)
        {
            Name = name;
            AvatarUrl = avatarUrl;
        }
    }

    public class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, UserDto>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<UpdateProfileCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public UpdateProfileCommandHandler(IUow uow, IAppLogger<UpdateProfileCommandHandler> logger, ICurrentUserService currentUserService)
        {
            _uow = uow;
            _logger = logger;
            _currentUserService = currentUserService;
        }


        public async Task<Result<UserDto>> Handle(UpdateProfileCommand command, CancellationToken ct)
        {
            string userId = _currentUserService.GetUser();
           

            _logger.LogInformation("Profile update started for UserId: {UserId}", userId);

            var user = await _uow.UserRepository.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("Profile update failed: User not found. UserId: {UserId}", userId);
                
                return UserErrors.NotFound(Guid.Parse(userId));
            }

            if (!string.IsNullOrWhiteSpace(command.Name))
            {
                user.Name = command.Name.Trim();
            }

            user.AvatarUrl = command.AvatarUrl;

            _uow.UserRepository.Update(user);

            _logger.LogInformation("Profile successfully updated for UserId: {UserId}", userId);

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
