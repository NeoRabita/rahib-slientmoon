using Microsoft.Extensions.Localization;
using SlientMoon.Application.Interfaces.Messaging;
using SlientMoon.SharedKernel.Resources;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Behaviours.Localization
{
    public class LocalizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IStringLocalizer<Messages> _localizer;

        public LocalizationBehavior(IStringLocalizer<Messages> localizer)
        {
            _localizer = localizer;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken ct, Func<Task<TResponse>> next)
        {
            var response = await next();

            if (response is Result<string> resultString && resultString.IsSuccess)
            {
                var localizedValue = _localizer[resultString.Value].Value;
                return (TResponse)(object)Result.Success(localizedValue);
            }

            if (response is string key)
            {
                var localizedValue = _localizer[key].Value;
                return (TResponse)(object)localizedValue;
            }

            return response;
        }
    }
}
