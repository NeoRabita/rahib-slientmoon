using SlientMoon.Application.Features.Pomodoros.Commands.CreatePomodoro;
using SlientMoon.Domain.Entities;
using System;

namespace SlientMoon.Application.Mappings
{
    public static class PomodoroMappingExtensions
    {
        public static Pomodoro ToPomodoro(this CreatePomodoroCommand command)
        {
            if (command == null) return null;

            return  new Pomodoro()
            {
                Name = command.Name,
                ShortBreakTime = command.ShortBreakTime,
                LongBreakTime = command.LongBreakTime,
                LongBreakInterval = command.LongBreakInterval,
                PeriodCount = command.PeriodCount,
                Color = command.Color,
                CreateDate = DateTime.Now,
                IsDeleted = false,
                PomodoroTime = command.PomodoroTime
            };
        }
    }
}
