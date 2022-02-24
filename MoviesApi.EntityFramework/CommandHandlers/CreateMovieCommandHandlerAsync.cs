using MoviesApi.Domain.Commands;
using MoviesAPI.CQS;

namespace MoviesApi.EntityFramework.CommandHandlers
{
    public class CreateMovieCommandHandlerAsync : ICommandHandlerAsync<CreateMovieCommand>
    {
        private readonly MoviesContext _context;

        public CreateMovieCommandHandlerAsync(MoviesContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(CreateMovieCommand command)
        {
            _context.Movies.Add(command.Movie);
            await _context.SaveChangesAsync();
        }
    }
}
