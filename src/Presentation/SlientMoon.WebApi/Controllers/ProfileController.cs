using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.DTOs.Profile;
using SlientMoon.Application.Features.Profile.Commands.UpdateProfile;
using SlientMoon.Application.Features.Profile.Queries.GetMe;
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : BaseController
    {
        [HttpGet("me")]
        public async Task<IResult> GetMe()
        {
            var authorizationHeader = HttpContext.Request.Headers.Authorization.ToString();

            var result = await Dispatcher.Send(new GetMeQuery(authorizationHeader));

            return HandleResult(result);
        }

        [HttpPatch("me")]
        public async Task<IResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var authorizationHeader = HttpContext.Request.Headers.Authorization.ToString();

            var command = new UpdateProfileCommand(authorizationHeader, request.Name, request.AvatarUrl);

            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }
    }
}
