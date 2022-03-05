using Microsoft.AspNetCore.Mvc;
using MoviesApi.Domain.Commands;
using MoviesApi.Domain.Entities;
using MoviesApi.Domain.Queries;
using MoviesAPI.CQS;
using MoviesAPI.Dtos;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("movies")]
    public class MoviesController : ControllerBase
    {
        private readonly ICommandHandlerAsync<CreateMovieCommand> _createMovieCommandHandlerAsync;
        private readonly IQueryHandlerAsync<GetAllMoviesQuery, IReadOnlyCollection<Movie>> _getAllMoviesQueryHandlerAsync;

        public MoviesController(
            ICommandHandlerAsync<CreateMovieCommand> createMovieCommandHandlerAsync,
            IQueryHandlerAsync<GetAllMoviesQuery, IReadOnlyCollection<Movie>> getAllMoviesQueryHandlerAsync)
        {
            _createMovieCommandHandlerAsync = createMovieCommandHandlerAsync;
            _getAllMoviesQueryHandlerAsync = getAllMoviesQueryHandlerAsync;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var getAllMoviesQuery = new GetAllMoviesQuery();
            var movies = await _getAllMoviesQueryHandlerAsync
                .HandleAsync(getAllMoviesQuery)
                .ConfigureAwait(false);

            var movieDtos = movies.Select(MovieDto.From);

            return Ok(movieDtos);
        }
    }
}
