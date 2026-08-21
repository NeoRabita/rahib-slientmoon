using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.Features.Tracks.Queries.StreamTrack;
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
    public class StreamingController : BaseController
    {
        [HttpGet("{id}/stream")]
        public async Task<IResult> StreamTrack([FromRoute] string id)
        {
            string? rangeHeader = Request.Headers.Range.ToString();

            var result = await Dispatcher.Send(new StreamTrackQuery(id, rangeHeader));

            return HandleResult(result);
        }
    }
}
