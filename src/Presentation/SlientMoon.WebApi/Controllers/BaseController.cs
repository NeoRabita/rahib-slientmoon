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
                // 1. MinIO-dan gələn Status Code-u təyin edirik (Məsələn: 206 və ya 200)
                Response.StatusCode = streamDto.StatusCode;

                // 2. Accept-Ranges header-ini əlavə edirik
                Response.Headers["Accept-Ranges"] = "bytes";

                // 3. Əgər MinIO Content-Range qaytarıbsa, cavaba yazırıq
                if (!string.IsNullOrEmpty(streamDto.ContentRange))
                {
                    Response.Headers["Content-Range"] = streamDto.ContentRange;
                }

                // 4. Content-Length-i təyin edirik
                if (streamDto.ContentLength.HasValue)
                {
                    Response.ContentLength = streamDto.ContentLength.Value;
                }

                // 5. Şəbəkə axınını (Stream) müştəriyə ötürürük
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