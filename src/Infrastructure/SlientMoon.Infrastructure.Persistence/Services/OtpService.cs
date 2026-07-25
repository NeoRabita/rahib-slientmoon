using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SlientMoon.Application.DTOs.Messages;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Enums;
using SlientMoon.Infrastructure.Persistence.Settings;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class OtpService : IOtpService
    {
        private readonly IDistributedCache _cache;
        private readonly IMessagePublisher _messagePublisher;
        private readonly RabbitMqSettings _rabbitMqSettings;

        private const string KeyPrefix = "otp:";

        private const int OtpExpirationMinutes = 10;

        private const int MaxAttempts = 5;

        public OtpService(IDistributedCache cache, IMessagePublisher messagePublisher, IOptions<APIAppSettings> apiSettings)
        {
            _cache = cache;
            _messagePublisher = messagePublisher;
            _rabbitMqSettings = apiSettings.Value.RabbitMq;
        }

        public async Task<string> GenerateOtpAsync(string userId, string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            var otpData = new OtpData
            {
                Code = otp,
                Attempts = 0,
                ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpirationMinutes),
            };


            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(OtpExpirationMinutes)
            };

            var key = $"{KeyPrefix}{userId}";
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(otpData), options);

            await _messagePublisher.PublishAsync(new NotificationMessage
            {
                Type = NotificationType.Email,
                To = email,
                Body = otp,
            }, _rabbitMqSettings.ConsumerQueue);

            return otp;
        }

        public async Task<Result> ValidateOtpAsync(string userId, string otp)
        {
            var key = $"{KeyPrefix}{userId}";

            var cachedOtpData = await _cache.GetStringAsync(key);

            if (cachedOtpData is null)
                return Result.Failure(Error.NotFound(
                    "OTP_NOT_FOUND",
                    "No pending OTP found. Please request a new one"
                    ));

            var otpData = JsonSerializer.Deserialize<OtpData>(cachedOtpData);

            if (otpData.Attempts >= MaxAttempts)
            {
                await _cache.RemoveAsync(key);

                return Result.Failure(Error.Failure(
                    "RATE_LIMIT_EXCEEDED",
                    "You have exceeded the maximum number of attempts. Please request a new OTP."
                ));
            }


            if (otpData.Code != otp)
            {
                otpData.Attempts++;
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = otpData.ExpiresAt
                };
                await _cache.SetStringAsync(key, JsonSerializer.Serialize(otpData), options);

                return Result.Failure(Error.Validation(
                    "INVALID_OTP",
                      $"The OTP code is incorrect. {MaxAttempts - otpData.Attempts} attempts remaining"
                ));
            }

            await _cache.RemoveAsync(key);
            return Result.Success();

        }

        public async Task RemoveOtpAsync(string userId)
        {
            var key = $"{KeyPrefix}{userId}";
            await _cache.RemoveAsync(key);
        }

    }
}
