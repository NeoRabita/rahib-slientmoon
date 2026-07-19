using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Account;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Auth.Commands.VerifyEmail
{
    public partial class VerifyEmailCommand : ICommand<AuthenticationResponse>
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }

    public class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand, AuthenticationResponse>
    {
        private readonly IOtpService _otpService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUow _uow;
        private readonly IAppLogger<VerifyEmailCommandHandler> _logger;

        public VerifyEmailCommandHandler(
            IOtpService otpService,
            IJwtTokenService jwtTokenService,
            IUow uow,
            IAppLogger<VerifyEmailCommandHandler> logger)
        {
            _otpService = otpService;
            _jwtTokenService = jwtTokenService;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<AuthenticationResponse>> Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("VerifyEmail started. Email: {Email}", command.Email);

            var user = await _uow.UserRepository.GetByEmailAsync(command.Email);
            if (user is null)
            {
                _logger.LogWarning("VerifyEmail failed: user not found. Email: {Email}", command.Email);

                return UserErrors.NotFoundByEmail;
            }

            if (user.EmailVerified)
            {
                _logger.LogWarning("VerifyEmail failed: email already verified. Email: {Email}", command.Email);
                
                return UserErrors.EmailAlreadyVerified;
            }

            var otpResult = await _otpService.ValidateOtpAsync(user.Id, command.Otp);
            if (otpResult.IsFailure)
            {
                _logger.LogWarning("VerifyEmail failed: invalid OTP. Email: {Email}", command.Email);

                return otpResult.Error;
            }

            user.EmailVerified = true;

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            

            _logger.LogInformation("VerifyEmail successful. Email: {Email}", command.Email);

            return Result.Success(new AuthenticationResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });

        }
    }
}
