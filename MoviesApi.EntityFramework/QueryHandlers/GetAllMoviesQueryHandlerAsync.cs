using Microsoft.EntityFrameworkCore;
using MoviesApi.Domain.Entities;
using MoviesApi.Domain.Queries;
using MoviesAPI.CQS;

namespace MoviesApi.EntityFramework.QueryHandlers
{
    public class GetAllMoviesQueryHandlerAsync : IQueryHandlerAsync<GetAllMoviesQuery, IReadOnlyCollection<Movie>>
    {
        private readonly MoviesContext _context;

        public GetAllMoviesQueryHandlerAsync(MoviesContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Movie>> HandleAsync(GetAllMoviesQuery query)
        {
            return await _context.Movies
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
