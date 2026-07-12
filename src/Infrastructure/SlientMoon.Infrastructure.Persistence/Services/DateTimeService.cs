using System;
using SlientMoon.Application.Interfaces.Services;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTimeOffset localTime => TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimeZoneInfo.FindSystemTimeZoneById("Azerbaijan Standard Time"));
        public DateTime NowUtc => localTime.DateTime;
    }
}