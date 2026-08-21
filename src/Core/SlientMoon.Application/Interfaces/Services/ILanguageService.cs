using SlientMoon.Domain.Enums;

namespace SlientMoon.Application.Interfaces.Services
{
    public interface ILanguageService
    {
        LanguageCode ValidateLanguage(string? language);
    }
}
