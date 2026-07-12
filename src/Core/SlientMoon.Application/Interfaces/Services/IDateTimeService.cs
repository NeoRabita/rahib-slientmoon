using System;

namespace SlientMoon.Application.Interfaces.Services
{
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
        DateTimeOffset localTime { get; }
    }
}