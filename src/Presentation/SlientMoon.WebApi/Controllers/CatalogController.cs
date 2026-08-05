using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.Features.Categories.Queries.GetCategories;
using SlientMoon.Domain.Enums;
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
    public class CatalogController : BaseController
    {
        [HttpGet("categories")]
        public async Task<IResult> GetCategories([FromQuery] string? type)
        {
            var result = await Dispatcher.Send(new GetCategoriesQuery(type));
            return HandleResult(result);
        }
    }
}
