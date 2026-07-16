using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.Features.Topics.Queries.GetAllTopics;
using SlientMoon.Application.Features.Topics.Queries.GetUserTopics;
using SlientMoon.Application.Features.Topics.Commands.UpdateUserTopics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class OnboardingController : BaseController
    {
        [HttpGet("topics")]
        public async Task<IResult> GetAllTopics()
        {
            var result = await Dispatcher.Send(new GetAllTopicsQuery());

            return HandleResult(result);
        }

        [HttpGet("me/topics")]
        public async Task<IResult> GetMyTopics()
        {
            var authorizationHeader = HttpContext.Request.Headers.Authorization.ToString();

            var query = new GetUserTopicsQuery(authorizationHeader);
            var result = await Dispatcher.Send(query);

            return HandleResult(result);
        }

        [HttpPut("me/topics")]
        public async Task<IResult> UpdateMyTopics([FromBody] List<string> topicIds)
        {
            var authorizationHeader = HttpContext.Request.Headers.Authorization.ToString();

            var command = new UpdateUserTopicsCommand(authorizationHeader, topicIds);
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }
    }
}