using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.Features.Auth.Commands.ForgotPassword;
using SlientMoon.Application.Features.Auth.Commands.GoogleLogin;
using SlientMoon.Application.Features.Auth.Commands.Login;
using SlientMoon.Application.Features.Auth.Commands.Logout;
using SlientMoon.Application.Features.Auth.Commands.Refresh;
using SlientMoon.Application.Features.Auth.Commands.Register;
using SlientMoon.Application.Features.Auth.Commands.ResendOtp;
using SlientMoon.Application.Features.Auth.Commands.ResetPassword;
using SlientMoon.Application.Features.Auth.Commands.VerifyEmail;
using SlientMoon.WebApi.StartupInjections.Validations;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
    [ValidateModel]
    public class AccountController : BaseController
    {

        [HttpPost("register")]
        public async Task<IResult> RegisterAsync([FromBody] RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await Dispatcher.Send(command, cancellationToken);

            return HandleResult(result);
        }

        [HttpPost("login")]
        public async Task<IResult> LoginAsync([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await Dispatcher.Send(command, cancellationToken);

            return HandleResult(result);
        }

        [HttpPost("verify-email")]
        public async Task<IResult> VerifyEmailAsync([FromBody] VerifyEmailCommand command, CancellationToken cancellationToken)
        {
            var result = await Dispatcher.Send(command, cancellationToken);

            return HandleResult(result);
        }

        [HttpPost("resend-otp")]
        public async Task<IResult> ResendOtpAsync([FromBody] ResendOtpCommand command, CancellationToken cancellationToken)
        {
            var result = await Dispatcher.Send(command, cancellationToken);

            return HandleResult(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IResult> ForgotPasswordAsync([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await Dispatcher.Send(command, cancellationToken);

            return HandleResult(result);
        }

        [HttpPost("reset-password")]
        public async Task<IResult> ResetPasswordAsync([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await Dispatcher.Send(command, cancellationToken);

            return HandleResult(result);
        }

        [HttpPost("refresh")]
        public async Task<IResult> RefreshAsync([FromBody] RefreshCommand command, CancellationToken cancellationToken)
        {
            var result = await Dispatcher.Send(command, cancellationToken);

            return HandleResult(result);
        }

        [HttpPost("logout")]
        public async Task<IResult> LogoutAsync([FromBody] LogoutCommand command, CancellationToken cancellationToken)
        {
            var result = await Dispatcher.Send(command, cancellationToken);

            return HandleResult(result);
        }

        [HttpPost("oauth/google")]
        public async Task<IResult> GoogleLoginAsync([FromBody] GoogleLoginCommand command, CancellationToken cancellationToken)
        {
            var result = await Dispatcher.Send(command, cancellationToken);

            return HandleResult(result);
        }
    }
}