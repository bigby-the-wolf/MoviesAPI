using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Domain.Entities;
using MoviesApi.Domain.Queries;
using MoviesAPI.CQS;

namespace MoviesApi.EntityFramework.QueryHandlers
{
    public class GetMovieByIdQueryHandlerAsync : IQueryHandlerAsync<GetMovieByIdQuery, Maybe<Movie>>
    {
        private readonly MoviesContext _context;

        public GetMovieByIdQueryHandlerAsync(MoviesContext context)
        {
            _context = context;
        }

        public async Task<Maybe<Movie>> HandleAsync(GetMovieByIdQuery query)
        {
            var movie = await _context.Movies
                .AsNoTracking()
                .Where(m => m.Id == query.Id)
                .SingleOrDefaultAsync();

#pragma warning disable CS8604 // Possible null reference argument.
            return movie;
#pragma warning restore CS8604 // Possible null reference argument.
        }
    }
}
