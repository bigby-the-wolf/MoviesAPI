using Functional.Maybe;
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
                return Maybe<Movie>.Nothing;
            if (string.IsNullOrEmpty(Description))
                return Maybe<Movie>.Nothing;

            var id = Id ?? Guid.NewGuid();
            return new Movie(id, Name, Description).ToMaybe();
        }
    }
}
