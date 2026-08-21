using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Common;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly IUow _uow;

        public TranslationService(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Dictionary<string, string>> GetTranslationsAsync(IEnumerable<string> keys, LanguageCode languageCode, CancellationToken cancellationToken = default)
        {
            var keyList = keys.Distinct().ToList();

            if(!keyList.Any())
                return new Dictionary<string, string>();

            var allTranslations = await _uow.GenericRepository<Translation>().GetAllAsync();

            return allTranslations
                .Where(t => t.LanguageCode == languageCode && keyList.Contains(t.Key))
                .ToDictionary(t => t.Key, t => t.Value);
        }

        public async Task SaveTranslationsAsync<TEntity>(string entityId, LanguageCode languageCode, Dictionary<string, string> propertyValues, CancellationToken cancellationToken = default) where TEntity : BaseEntity
        {
            var keys = propertyValues.Keys
                .Select(propName => TranslationExtensions.BuildKey<TEntity>(propName, entityId))
                .ToList();

            var existingTranslations = await _uow.GenericRepository<Translation>().GetAllAsync();
            var dbTranslations = existingTranslations
                .Where(t => t.LanguageCode == languageCode && keys.Contains(t.Key))
                .ToDictionary(t => t.Key);

            foreach (var (propertyName, value) in propertyValues)
            {
                var key = TranslationExtensions.BuildKey<TEntity>(propertyName, entityId);

                if (dbTranslations.TryGetValue(key, out var existingTranslation))
                {
                    existingTranslation.Value = value;
                }
                else
                {
                    var newTranslation = new Translation
                    {
                        Id = Guid.NewGuid().ToString(),
                        Key = key,
                        LanguageCode = languageCode,
                        Value = value
                    };
                    await _uow.GenericRepository<Translation>().AddAsync(newTranslation);
                }
            }
        }
    }
}
