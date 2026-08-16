using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SlientMoon.Application.DTOs.Storage;
using SlientMoon.WebApi.Services;

namespace SlientMoon.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BaseController() : ControllerBase
    {
        private IProblemResultFactory _problemFactory;
        protected IProblemResultFactory ProblemFactory =>
            _problemFactory ??= HttpContext.RequestServices.GetRequiredService<IProblemResultFactory>();
        private IDispatcher _dispatcher;
        protected IDispatcher Dispatcher =>
            _dispatcher ??= HttpContext.RequestServices.GetRequiredService<IDispatcher>();

        protected IResult HandleResult<T>(Result<T> result)
        {
            if (!result.IsSuccess)
                return ProblemFactory.CreateProblem(result);

            if (result.Value is TrackStreamDto streamDto)
            {
                long start = streamDto.Offset ?? 0;
                long chunkLength = streamDto.Length;
                long end = start + chunkLength - 1;
                long total = streamDto.TotalSize;

                if (end >= total)
                {
                    end = total - 1;
                }

                Response.Headers["Accept-Ranges"] = "bytes";
                Response.StatusCode = StatusCodes.Status206PartialContent; // HƏMİŞƏ 206
                Response.Headers["Content-Range"] = $"bytes {start}-{end}/{total}";
                Response.ContentLength = chunkLength; // Yalnız 1MB-lıq hissənin ölçüsü

                return Results.Stream(
                    stream: streamDto.Stream,
                    contentType: streamDto.ContentType
                );
            }

            return Results.Ok(result.Value);
        }

        protected IResult HandleResult(Result result) =>
            result.IsSuccess ? Results.Ok() : ProblemFactory.CreateProblem(result);
    }
}