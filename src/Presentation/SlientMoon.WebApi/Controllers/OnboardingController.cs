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
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
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
            var result = await Dispatcher.Send(new GetUserTopicsQuery());

            return HandleResult(result);
        }

        [HttpPut("me/topics")]
        public async Task<IResult> UpdateMyTopics([FromBody] UpdateUserTopicsCommand command)
        {
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }

        [HttpGet("me/reminders")]
        public async Task<IResult> GetUserReminders()
        {
            var result = await Dispatcher.Send(new GetUserRemindersQuery());

            return HandleResult(result);
        }

        [HttpPost("me/reminders")]
        public async Task<IResult> CreateReminder([FromBody] CreateReminderCommand command)
        {
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }

        // Burda nece olacaq result 

        [HttpPatch("me/reminders/{id}")]
        public async Task<IResult> UpdateReminder([FromRoute] string id, [FromBody] UpdateReminderRequest request)
        {
            var result = await Dispatcher.Send(new UpdateReminderCommand(id, request));

            return HandleResult(result);
        }

        [HttpDelete("me/reminders/{id}")]
        public async Task<IResult> DeleteReminder([FromRoute] string id)
        {
            var result = await Dispatcher.Send(new DeleteReminderCommand(id));

            return HandleResult(result);
        }
    }
}