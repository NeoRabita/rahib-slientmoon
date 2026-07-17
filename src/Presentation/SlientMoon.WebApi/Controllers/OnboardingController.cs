using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.DTOs.Reminders;
using SlientMoon.Application.Features.Reminders.Commands.CreateReminder;
using SlientMoon.Application.Features.Reminders.Commands.DeleteReminder;
using SlientMoon.Application.Features.Reminders.Commands.UpdateReminder;
using SlientMoon.Application.Features.Reminders.Queries.GetMyReminders;
using SlientMoon.Application.Features.Topics.Commands.UpdateUserTopics;
using SlientMoon.Application.Features.Topics.Queries.GetAllTopics;
using SlientMoon.Application.Features.Topics.Queries.GetUserTopics;
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

        [HttpGet("me/reminders")]
        public async Task<IResult> GetUserReminders([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            var query = new GetUserRemindersQuery(authorizationHeader);
            var result = await Dispatcher.Send(query);

            return HandleResult(result);
        }

        [HttpPost("me/reminders")]
        public async Task<IResult> CreateReminder([FromBody] CreateReminderRequest request)
        {
            var command = new CreateReminderCommand(request);
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }

        [HttpPatch("me/reminders/{id}")]
        public async Task<IResult> UpdateReminder([FromRoute] string id, [FromBody] UpdateReminderRequest request)
        {
            var authorizationHeader = HttpContext.Request.Headers.Authorization.ToString();

            var command = new UpdateReminderCommand(id, authorizationHeader, request);
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }

        [HttpDelete("me/reminders/{id}")]
        public async Task<IResult> DeleteReminder([FromRoute] string id)
        {
            var authorizationHeader = HttpContext.Request.Headers.Authorization.ToString();

            var command = new DeleteReminderCommand(id, authorizationHeader);
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }
    }
}