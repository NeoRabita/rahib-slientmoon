using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Account;
using SlientMoon.Application.DTOs.Email;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Auth.Commands.Register
{
    public partial class RegisterCommand : ICommand<RegisterResponse>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }


    public class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegisterResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IUow _uow;
        private readonly IAppLogger<RegisterCommandHandler> _logger;
        
        public RegisterCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IEmailService emailService,
            IOtpService otpService,
            IMessagePublisher messagePublisher,
            IUow uow,
            IAppLogger<RegisterCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _otpService = otpService;
            _messagePublisher = messagePublisher;
            _emailService = emailService;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<RegisterResponse>> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Register started. Email: {Email}", command.Email);

            if (command == null)
                return Error.NullValue;

            var existingUser = await _userRepository.GetByEmailAsync(command.Email);
            if(existingUser != null)
            {
                _logger.LogWarning("Register failed: email already exists. Email: {Email}", command.Email);

                return UserErrors.EmailNotUnique;
            }


            var user = new ApplicationUser
            {
                Name = command.Name,
                Email = command.Email,
                PasswordHash = _passwordHasher.Hash(command.Password),
                EmailVerified = false,
                LoginType = LoginType.Normal,
            };

            await _uow.UserRepository.AddAsync(user);

            var otp = await _otpService.GenerateOtpAsync(user.Id);

            var emailRequest = new EmailRequest
            {
                To = user.Email,
                Subject = "SilentMoon - E-poçt Təsdiqləmə Kodu",
                Body = otp
            };

            await _messagePublisher.PublishAsync(emailRequest, "email.otp.send");


            _logger.LogInformation("Register successful. OTP generated. UserId: {UserId}", user.Id);

            return new RegisterResponse
            {
                Message = "Registration successful.",
                Email = command.Email,
                OtpExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };
        }
    }
}
