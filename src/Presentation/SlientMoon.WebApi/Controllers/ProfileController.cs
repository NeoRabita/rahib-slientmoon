using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.Features.Profile.Commands.UpdateProfile;
using SlientMoon.Application.Features.Profile.Queries.GetMe;
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ProfileController : BaseController
    {
        [HttpGet("me")]
        public async Task<IResult> GetMe()
        {
            var result = await Dispatcher.Send(new GetMeQuery());

            return HandleResult(result);
        }

        [HttpPatch("me")]
        public async Task<IResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }
    }
}
