using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.DTOs.Courses;
using SlientMoon.Application.DTOs.Favorites;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Errors;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Favorites.Commands.AddFavorite
{
    public class AddFavoriteCommand : ICommand<FavoriteDto>
    {
        public string CourseId { get; set; }

        public AddFavoriteCommand(string courseId)
        {
            CourseId = courseId;
        }
    }

    public class AddFavoriteCommandHandler : ICommandHandler<AddFavoriteCommand, FavoriteDto>
    {
        private readonly IUow _uow;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAppLogger<AddFavoriteCommandHandler> _logger;
        private readonly IDateTimeService _dateTimeService;

        public AddFavoriteCommandHandler(
            IUow uow,
            ICurrentUserService currentUserService,
            IAppLogger<AddFavoriteCommandHandler> logger,
            IDateTimeService dateTimeService)
        {
            _uow = uow;
            _currentUserService = currentUserService;
            _logger = logger;
            _dateTimeService = dateTimeService;
        }

        public async Task<Result<FavoriteDto>> Handle(AddFavoriteCommand command, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
            {
                return UserErrors.Unauthorized();
            }

            string userId = _currentUserService.UserId;

            var course = await _uow.CourseRepository.GetByIdAsync(command.CourseId);
            if (course == null)
            {
                return FavoriteErrors.CourseNotFound;
            }

            bool alreadyExists = await _uow.FavoriteRepository.GetAllAsFavorites()
                .AnyAsync(f => f.UserId == userId && f.CourseId == command.CourseId, ct);

            if (alreadyExists)
            {
                return FavoriteErrors.AlreadyExists;
            }

            var favorite = new Favorite
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                CourseId = command.CourseId,
                CreatedAt = _dateTimeService.NowUtc
            };

            await _uow.FavoriteRepository.AddFavoriteAsync(favorite);

            _logger.LogInformation("Favorite added. UserId: {UserId}, CourseId: {CourseId}", userId, command.CourseId);


            var resultDto = new FavoriteDto
            {
                Id = favorite.Id,
                CourseId = favorite.CourseId,
                CreatedAt = favorite.CreatedAt,
                Course = new CourseDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Subtitle = course.Subtitle,
                    Type = course.Type.ToString().ToLower(),
                    CategoryId = course.CategoryId,
                    ImageUrl = course.ImageUrl,
                    DurationSec = course.DurationSec,
                    IsFeatured = course.IsFeatured,
                    Narrators = course.CourseNarrators?
                        .Select(cn => cn.Narrator.Gender.ToString().ToLower())
                        .ToList()
                }
            };

            return resultDto;
        }
    }
}
