using Application.Abstractions.Messaging;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommand : ICommand<string>
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
    }

    public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, string>
    {
        private readonly IUow _uow;
        private readonly IOtpService _otpService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAppLogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(
            IUow uow,
            IOtpService otpService,
            IPasswordHasher passwordHasher,
            IAppLogger<ResetPasswordCommandHandler> logger)
        {
            _uow = uow;
            _otpService = otpService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reset password attempt for Email: {Email}", command.Email);

            var user = await _uow.UserRepository.GetByEmailAsync(command.Email);
            if (user is null)
            {
                _logger.LogWarning("Reset password failed: User not found. Email: {Email}", command.Email);
                return UserErrors.InvalidCredentials;
            }

            var isOtpValid = await _otpService.ValidateOtpAsync(user.Id, command.Otp);
            if (isOtpValid.IsFailure)
            {
                _logger.LogWarning("Reset password failed: Invalid or expired OTP. Email: {Email}", command.Email);
                return UserErrors.InvalidCredentials;
            }

            user.PasswordHash = _passwordHasher.Hash(command.NewPassword);

            user.RefreshToken = null;
            user.RefreshTokenId = null;

            _uow.UserRepository.Update(user);

            _logger.LogInformation("Password successfully reset for UserId: {UserId}", user.Id);

            return "Şifrəniz uğurla yeniləndi. Yeni şifrənizlə daxil ola bilərsiniz.";
        }
    }
}