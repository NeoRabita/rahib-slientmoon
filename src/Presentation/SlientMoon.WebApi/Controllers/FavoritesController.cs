using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.Features.Favorites.Commands.AddFavorite;
using SlientMoon.Application.Features.Favorites.Commands.RemoveFavorite;
using SlientMoon.Application.Features.Favorites.Queries.GetMyFavorites;
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
    public class FavoritesController : BaseController
    {
        [HttpGet]
        public async Task<IResult> GetMyFavorites([FromQuery] GetMyFavoritesQuery query)
        {
            var result = await Dispatcher.Send(query);

            return HandleResult(result);
        }

        [HttpPost]
        public async Task<IResult> AddFavorite([FromBody] AddFavoriteCommand command)
        {
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IResult> RemoveFavorite([FromRoute] string id)
        {
            var command =  new RemoveFavoriteCommand(id);
           
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }
    }
}
