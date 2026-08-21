using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.Features.Admin.Courses.Commands.SaveCourseTranslation;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
    public class AdminCoursesController : BaseController
    {
        [HttpPost("translations")]
        public async Task<IResult> SaveTranslationAsync(
            [FromBody] SaveCourseTranslationCommand command,
            CancellationToken cancellationToken)
        {
            var result = await Dispatcher.Send(command, cancellationToken);

            return HandleResult(result);
        }
    }
}
