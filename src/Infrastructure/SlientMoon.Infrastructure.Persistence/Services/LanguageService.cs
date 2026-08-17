using SlientMoon.Application.Interfaces.Services;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class LanguageService : ILanguageService
    {
        public string ValidateLanguage(string? lang)
        {
            if (string.IsNullOrWhiteSpace(lang))
                return "en";

            var cleanLang = lang.Split(',')[0].Split('-')[0].Trim().ToLower();

            return cleanLang switch
            {
                "az" => "az",
                "ru" => "ru",
                _ => "en"
            };
        }
    }
}
