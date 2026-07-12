using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.DTOs.Account;
using SlientMoon.Application.DTOs.Email;
using SlientMoon.Application.Features.Auth.Commands.GoogleLogin;
using SlientMoon.Application.Features.Auth.Commands.Login;
using SlientMoon.Application.Features.Auth.Commands.Logout;
using SlientMoon.Application.Features.Auth.Commands.Refresh;
using SlientMoon.Application.Features.Auth.Commands.Register;
using SlientMoon.Application.Features.Auth.Commands.ResendOtp;
using SlientMoon.Application.Features.Auth.Commands.VerifyEmail;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.WebApi.StartupInjections.Validations;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [ValidateModel]
    public class AccountController : BaseController
    {

        private readonly IDispatcher _dispatcher;

        public AccountController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password
            };

            var result = await _dispatcher.Send(command, cancellationToken);


            if (result.IsFailure)
                return BadRequest(result.Error);

            return StatusCode(201, result.Value);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await _dispatcher.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmailAsync([FromBody] VerifyEmailRequest request, CancellationToken cancellationToken)
        {
            var command = new VerifyEmailCommand
            {
                Email = request.Email,
                Otp = request.Otp
            };

            var result = await _dispatcher.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);


            return Ok(result.Value);
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtpAsync([FromBody] ResendOtpRequest request, CancellationToken cancellationToken)
        {
            var command = new ResendOtpCommand 
            { 
                Email = request.Email
            };

            var result = await _dispatcher.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshRequest request, CancellationToken cancellationToken)
        {
            var command = new RefreshCommand
            {
                RefreshToken = request.RefreshToken,
            };

            var result = await Dispatcher.Send<AuthenticationResponse>(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest request, CancellationToken cancellationToken)
        {
            var command = new LogoutCommand
            {
                RefreshToken = request.RefreshToken
            };

            var result = await _dispatcher.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok();
        }

        [HttpPost("oauth/google")]
        public async Task<IActionResult> GoogleLoginAsync([FromBody] GoogleLoginRequest request, CancellationToken cancellationToken)
        {
            var command = new GoogleLoginCommand
            {
                IdToken = request.IdToken
            };

            var result = await _dispatcher.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}