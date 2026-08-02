using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Home;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Application.Mappings;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Home.Queries.GetHomeFeed
{
    public class GetHomeFeedQuery : IQuery<HomeDto>
    {

    }

    public class GetHomeFeedQueryHandler : IQueryHandler<GetHomeFeedQuery, HomeDto>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetHomeFeedQueryHandler> _logger;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICurrentUserService _currentUserService;

        public GetHomeFeedQueryHandler(
            IUow uow,
            IAppLogger<GetHomeFeedQueryHandler> logger,
            IDateTimeService dateTimeService,
            ICurrentUserService currentUserService)
        {
            _uow = uow;
            _logger = logger;
            _dateTimeService = dateTimeService;
            _currentUserService = currentUserService;
        }

        public async Task<Result<HomeDto>> Handle(GetHomeFeedQuery query, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
            {
                return UserErrors.Unauthorized();
            }

            _logger.LogInformation("Home feed məlumatları sorğulanır.");

            var today = _dateTimeService.NowUtc.Date;

            var dailyThoughtEntity = await _uow.DailyThoughtRepository.GetDailyThoughtByDateAsync(today, ct);

            DailyThoughtDto? dailyThoughtDto = null;
            if (dailyThoughtEntity?.Course != null)
            {
                dailyThoughtDto = dailyThoughtEntity.ToDailyThoughtDto();
            }

            var allCourses = await _uow.CourseRepository.GetHomeFeedCoursesAsync(ct);

            var recommended = allCourses
                .Take(4)
                .Select(c => c.ToHomeCourseDto())
                .ToList();

            var featuredSleep = allCourses
                .Where(c => c.Type == CategoryType.Sleep && c.IsFeatured)
                .Take(4)
                .Select(c => c.ToHomeCourseDto())
                .ToList();

            var popularMeditations = allCourses
                .Where(c => c.Type == CategoryType.Meditation)
                .OrderByDescending(c => c.ViewCount)
                .Take(4)
                .Select(c => c.ToHomeCourseDto())
                .ToList();

            var response = new HomeDto
            {
                Recommended = recommended,
                DailyThought = dailyThoughtDto,
                FeaturedSleep = featuredSleep,
                PopularMeditations = popularMeditations
            };

            return response;
        }
    }

}
