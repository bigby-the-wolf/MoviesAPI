using MoviesApi.Domain.Entities;
using MoviesAPI.CQS;

namespace MoviesApi.Domain.Queries
{
    public class GetAllMoviesQuery : IQuery<IReadOnlyCollection<Movie>>
    {
    }
}
