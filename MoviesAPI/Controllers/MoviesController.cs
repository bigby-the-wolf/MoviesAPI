using Microsoft.AspNetCore.Mvc;
using MoviesApi.Domain.Commands;
using MoviesApi.Domain.Entities;
using MoviesAPI.CQS;
using MoviesAPI.Dtos;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("movies")]
    public class MoviesController : ControllerBase
    {
        public MoviesController()
        {
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MovieDto movieDto)
        {
            if (movieDto is null)
                throw new ArgumentNullException(nameof(movieDto));

            var id = Guid.NewGuid();

            if (!movieDto.Parse(id).HasValue)
                return BadRequest();

            await Task.Delay(1000).ConfigureAwait(false);

            return Ok(id);
        }
    }
}
