using CSharpFunctionalExtensions;
using MoviesApi.Domain.Entities;

namespace MoviesAPI.Dtos
{
    public class MovieDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public Maybe<Movie> Parse()
        {
            if (string.IsNullOrEmpty(Name))
                return Maybe<Movie>.None;
            if (string.IsNullOrEmpty(Description))
                return Maybe<Movie>.None;

            var id = Id ?? Guid.NewGuid();
            return new Movie(id, Name, Description);
        }

        public static MovieDto From(Movie movie)
        {
            if (movie == null)
                throw new ArgumentNullException(nameof(movie));

            return new MovieDto { Id = movie.Id, Name = movie.Name, Description = movie.Description };
        }
    }
}
