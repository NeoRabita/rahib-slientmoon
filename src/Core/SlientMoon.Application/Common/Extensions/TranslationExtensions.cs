using SlientMoon.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SlientMoon.Application.Common.Extensions
{
    public static class TranslationExtensions
    {
        public static string BuildKey<TEntity>(string propertyName, string id)
        {
            var entityName = typeof(TEntity).Name;

            return $"{entityName}_{propertyName}_{id}";
        }

        public static IEnumerable<string> GetTranslationKeys<TEntity>(
            this IEnumerable<TEntity> entities,
            params string[] propertyNames)
            where TEntity : BaseEntity
        {
            return entities.SelectMany(e => propertyNames.Select(prop => BuildKey<TEntity>(prop, e.Id)));
        }

        public static string GetTranslation<TEntity>(
            this TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression,
            Dictionary<string, string>? translations)
            where TEntity : BaseEntity
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                var propertyName = memberExpression.Member.Name;
                var key = BuildKey<TEntity>(propertyName, entity.Id);

                if (translations != null && translations.TryGetValue(key, out var translatedValue))
                {
                    return translatedValue;
                }

                var compiled = propertyExpression.Compile();
                return compiled(entity) ?? string.Empty;
            }

            return string.Empty;
        }
    }
}