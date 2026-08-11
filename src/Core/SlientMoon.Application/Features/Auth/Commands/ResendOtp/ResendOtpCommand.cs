using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Account;
using SlientMoon.Application.DTOs.Email;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Auth.Commands.ResendOtp
{
    public partial class ResendOtpCommand : ICommand<ResendOtpResponse>
    {
        public string Email { get; set; }
    }

    public class ResendOtpCommandHandler : ICommandHandler<ResendOtpCommand, ResendOtpResponse>
    {
        private readonly IOtpService _otpService;
        private readonly IUow _uow;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IEmailService _emailService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IAppLogger<ResendOtpCommandHandler> _logger;


        public ResendOtpCommandHandler(
            IOtpService otpService,
            IEmailService emailService,
            IMessagePublisher messagePublisher,
            IUow uow,
            IDateTimeService dateTimeService,
            IAppLogger<ResendOtpCommandHandler> logger)
        {
            _otpService = otpService;
            _emailService = emailService;
            _messagePublisher = messagePublisher;
            _uow = uow;
            _logger = logger;
            _dateTimeService = dateTimeService;
        }

        public async Task<Result<ResendOtpResponse>> Handle(ResendOtpCommand command, CancellationToken ct)
        {
            _logger.LogInformation("ResendOtp started. Email: {Email}", command.Email);

            var user = await _uow.UserRepository.GetByEmailAsync(command.Email);
            if (user is null)
            {
                _logger.LogWarning("ResendOtp failed: user not found. Email: {Email}", command.Email);
                return UserErrors.NotFoundByEmail;
            }

            if (user.EmailVerified)
            {
                _logger.LogWarning("ResendOtp failed: email already verified. Email: {Email}", command.Email);
                return UserErrors.EmailAlreadyVerified;
            }


            await _otpService.RemoveOtpAsync(user.Id);

            var otp = await _otpService.GenerateOtpAsync(user.Id, user.Email);


            return new ResendOtpResponse
            {
                Message = "New OTP code has been sent to your email.",
                OtpExpiresAt = _dateTimeService.NowUtc.AddMinutes(10)
            };

        }
    }

}
