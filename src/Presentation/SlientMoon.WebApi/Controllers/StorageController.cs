using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.Features.Storage.Commands.UploadFile;
using SlientMoon.Domain.Enums;
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
    public class StorageController : BaseController
    {
        [HttpPost("upload")]
        public async Task<IResult> Upload(IFormFile file, [FromForm] StorageType storageType)
        {
            await using var stream = file.OpenReadStream();

            var command = new UploadFileCommand(
                stream,
                file.FileName,
                file.ContentType,
                storageType
            );

            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }
    }
}
