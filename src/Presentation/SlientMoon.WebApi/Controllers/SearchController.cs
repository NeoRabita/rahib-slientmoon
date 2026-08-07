using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.DTOs.Search;
using SlientMoon.Application.Features.Search.Queries.Search;
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
    public class SearchController : BaseController
    {

        [HttpGet("search")]

        public async Task<IResult> Search(
            [FromQuery] string q, string? type = null, int page = 1, int limit = 20)
        {
            var result = await Dispatcher.Send(new SearchQuery(q, type, page, limit));
            return HandleResult(result);
        }
    }
}
