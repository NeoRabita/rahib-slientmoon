using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.Features.Categories.Queries.GetCategories;
using SlientMoon.Application.Features.Courses.Queries.GetCourseById;
using SlientMoon.Application.Features.Courses.Queries.GetCourses;
using SlientMoon.Application.Features.Courses.Queries.GetCourseTracks;
using SlientMoon.Application.Features.Courses.Queries.GetRelatedCourses;
using SlientMoon.Application.Features.Tracks.Queries.GetTrackById;
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

        [HttpGet("courses")]
        public async Task<IResult> GetCourses([FromQuery] GetCoursesQuery query)
        {
            var result = await Dispatcher.Send(query);
            return HandleResult(result);
        }

        [HttpGet("courses/{id}")]
        public async Task<IResult> GetCourseById([FromRoute] string id)
        {
            var result = await Dispatcher.Send(new GetCourseByIdQuery(id));
            return HandleResult(result);
        }

        [HttpGet("courses/{id}/tracks")]
        public async Task<IResult> GetCourseTracks([FromRoute] string id, [FromQuery] Gender? narrator)
        {
            var result = await Dispatcher.Send(new GetCourseTracksQuery(id, narrator));
            return HandleResult(result);
        }

        [HttpGet("courses/{id}/related")]
        public async Task<IResult> GetRelatedCourses([FromRoute] string id, [FromQuery] int limit = 20)
        {
            var result = await Dispatcher.Send(new GetRelatedCoursesQuery(id, limit));
            return HandleResult(result);
        }

        [HttpGet("tracks/{id}")]
        public async Task<IResult> GetTrackById([FromRoute] string id)
        {
            var result = await Dispatcher.Send(new GetTrackByIdQuery(id));
            return HandleResult(result);
        }
    }
}
