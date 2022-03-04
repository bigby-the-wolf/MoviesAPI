using Microsoft.AspNetCore.Mvc;
using MoviesApi.Domain.Commands;
using MoviesAPI.CQS;
using MoviesAPI.Dtos;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("movies")]
    public class MoviesController : ControllerBase
    {
        private readonly ICommandHandlerAsync<CreateMovieCommand> _createMovieCommandHandlerAsync;

        public MoviesController(ICommandHandlerAsync<CreateMovieCommand> createMovieCommandHandlerAsync)
        {
            _createMovieCommandHandlerAsync = createMovieCommandHandlerAsync;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MovieDto movieDto)
        {
            if (movieDto is null)
                throw new ArgumentNullException(nameof(movieDto));

            var movieMaybe = movieDto.Parse();
            if (!movieMaybe.HasValue)
                return BadRequest();

            var createMovieCommand = new CreateMovieCommand(movieMaybe.Value);
            await _createMovieCommandHandlerAsync
                .HandleAsync(createMovieCommand)
                .ConfigureAwait(false);

            return Ok(createMovieCommand.Movie.Id);
        }
    }
}
