using CSharpFunctionalExtensions;
using MoviesApi.Domain.Entities;
using MoviesAPI.CQS;

namespace MoviesApi.Domain.Queries
{
    public class GetMovieByIdQuery : IQuery<Maybe<Movie>>
    {
        public GetMovieByIdQuery(Guid id)
            => Id = id;

        public Guid Id { get; private set; }
    }
}
