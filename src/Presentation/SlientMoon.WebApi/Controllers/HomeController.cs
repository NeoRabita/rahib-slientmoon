using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.Features.Home.Queries.GetHomeFeed;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public async Task<IResult> GetHomeFeed(CancellationToken ct)
        {
            string? language = Request.Headers["Accept-Language"].ToString();

            var result = await Dispatcher.Send(new GetHomeFeedQuery(language), ct);

            return HandleResult(result);
        }
    }
}
