using Application.Abstractions.Messaging;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Favorites.Commands.RemoveFavorite
{
    public class RemoveFavoriteCommand : ICommand<bool>
    {

        public string Id { get; set; }

        public RemoveFavoriteCommand(string id)
        {
            Id = id;
        }
    }

    public class RemoveFavoriteCommandHandler : ICommandHandler<RemoveFavoriteCommand, bool>
    {
        private readonly IUow _uow;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAppLogger<RemoveFavoriteCommandHandler> _logger;

        public RemoveFavoriteCommandHandler(
            IUow uow,
            ICurrentUserService currentUserService,
            IAppLogger<RemoveFavoriteCommandHandler> logger)
        {
            _uow = uow;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(RemoveFavoriteCommand command, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
            {
                return UserErrors.Unauthorized();
            }

            var userId = _currentUserService.UserId;

            var favorite = await _uow.FavoriteRepository.GetByIdAsync(command.Id);
            if (favorite == null)
            {
                return FavoriteErrors.NotFound;
            }

            if(favorite.UserId != userId)
            {
                return FavoriteErrors.Forbidden;
            }

            _uow.FavoriteRepository.Delete(favorite);

            _logger.LogInformation("Favorite removed. Id: {FavoriteId}, UserId: {UserId}", command.Id, userId);

            return true;
        }
    }
}
