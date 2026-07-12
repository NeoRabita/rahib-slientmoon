using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlientMoon.Application.DTOs.ViewModels;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface IPomodoroRepository : IGenericRepository<Pomodoro>
    {
        public Task<IEnumerable<PomodoroViewModel>> GetUserPomodoros(string userId);
        public List<PomodoroColors> GetPomodoroColors();
        public Task<PomodoroDetailsViewModel> GetPomodoroDetails(string userId, string pomodoroId);
        public Task<string> CreatePomodoroLog(string pomodoroId);

    }
}