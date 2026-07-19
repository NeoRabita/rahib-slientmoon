using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Email;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : ICommand<string>
    {
        public string Email { get; set; }
    }

    public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, string>
    {
        private readonly IUow _uow;
        private readonly IOtpService _otpService;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IAppLogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(
            IUow uow,
            IOtpService otpService,
            IMessagePublisher messagePublisher,
            IAppLogger<ForgotPasswordCommandHandler> logger)
        {
            _uow = uow;
            _otpService = otpService;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Forgot password process started for Email: {Email}", command.Email);

            if (string.IsNullOrWhiteSpace(command.Email))
                return Error.NullValue;

            var user = await _uow.UserRepository.GetByEmailAsync(command.Email);


            if (user is null)
            {
                _logger.LogWarning("Forgot password requested for non-existing email: {Email}", command.Email);
                return "Şifrə sıfırlama kodu e-poçt ünvanınıza göndərildi.";
            }

            var otp = await _otpService.GenerateOtpAsync(user.Id);

            var emailRequest = new EmailRequest
            {
                To = user.Email,
                Subject = "SilentMoon - Şifrə Sıfırlama Kodu",
                Body = $"Şifrənizi sıfırlamaq üçün birdəfəlik kodunuz: {otp}. Bu kod 10 dəqiqə ərzində etibarlıdır."
            };


            await _messagePublisher.PublishAsync(emailRequest, "email.otp.send");

            _logger.LogInformation("Forgot password OTP successfully generated and queued for UserId: {UserId}", user.Id);

            return "Şifrə sıfırlama kodu e-poçt ünvanınıza göndərildi.";
        }
    }
}