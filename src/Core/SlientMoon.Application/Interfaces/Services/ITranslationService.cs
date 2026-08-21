using SlientMoon.Domain.Common;
using SlientMoon.Domain.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Services
{
    public interface ITranslationService
    {
        Task<Dictionary<string, string>> GetTranslationsAsync(
            IEnumerable<string> keys,
            LanguageCode languageCode,
            CancellationToken cancellationToken = default);

        Task SaveTranslationsAsync<TEntity>(
            string entityId,
            LanguageCode languageCode,
            Dictionary<string, string> propertyValues,
            CancellationToken cancellationToken = default) where TEntity : BaseEntity;
    }
}
