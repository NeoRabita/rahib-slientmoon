using Application.Abstractions.Messaging;
using Microsoft.Extensions.Localization;
using SlientMoon.Application.DTOs.Home;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Application.Mappings;
using SlientMoon.SharedKernel.Resources;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Home.Queries.GetHomeFeed
{
    public class GetHomeFeedQuery : IQuery<HomeDto>
    {
        public string? Language { get; set; }

        public GetHomeFeedQuery(string? language = null)
        {
            Language = language;
        }
    }

    public class GetHomeFeedQueryHandler : IQueryHandler<GetHomeFeedQuery, HomeDto>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetHomeFeedQueryHandler> _logger;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly ILanguageService _languageService;

        public GetHomeFeedQueryHandler(
            IUow uow,
            IAppLogger<GetHomeFeedQueryHandler> logger,
            IDateTimeService dateTimeService,
            ICurrentUserService currentUserService,
            IStringLocalizer<Messages> localizer,
            ILanguageService languageService)
        {
            _uow = uow;
            _logger = logger;
            _dateTimeService = dateTimeService;
            _currentUserService = currentUserService;
            _localizer = localizer;
            _languageService = languageService;
        }

        public async Task<Result<HomeDto>> Handle(GetHomeFeedQuery query, CancellationToken ct)
        {
            string userId = _currentUserService.GetUser();
            string currentLang = _languageService.ValidateLanguage(query.Language);

            _logger.LogInformation("Home feed məlumatları sorğulanır.");

            var user = await _uow.UserRepository.GetByIdAsync(userId, ct);
            string userName = user.Name ?? "User";
            var now = _dateTimeService.NowUtc;
            var hour = now.Hour;

            string greetingKey = hour switch
            {
                >= 5 and < 12 => "Greeting.Morning",
                >= 12 and < 18 => "Greeting.Afternoon",
                _ => "Greeting.Evening"
            };

            var greetingDto = new GreetingDto
            {
                Title = $"{_localizer[greetingKey]}, {userName}",
                Message = _localizer["Greeting.Submessage"].Value
            };
            var recommendedTitle = _localizer["Home.RecommendedForYou"].Value;


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
                .Select(c => c.ToHomeCourseDto(currentLang))
                .ToList();

            var featuredSleep = allCourses
                .Where(c => c.Category != null &&
                            c.Category.CategoryType != null &&
                            c.Category.CategoryType.Slug.ToLower() == "sleep" &&
                            c.IsFeatured)
                .Take(4)
                .Select(c => c.ToHomeCourseDto(currentLang))
                .ToList();

            var popularMeditations = allCourses
                .Where(c => c.Category != null &&
                            c.Category.CategoryType != null &&
                            c.Category.CategoryType.Slug.ToLower() == "meditation")
                .OrderByDescending(c => c.ViewCount)
                .Take(4)
                .Select(c => c.ToHomeCourseDto(currentLang))
                .ToList();

            var response = new HomeDto
            {
                Greeting = greetingDto,
                RecommendedTitle = recommendedTitle,
                Recommended = recommended,
                DailyThought = dailyThoughtDto,
                FeaturedSleep = featuredSleep,
                PopularMeditations = popularMeditations
            };

            return response;
        }
    }

}
