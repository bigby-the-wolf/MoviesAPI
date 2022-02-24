using Functional.Maybe;
using MoviesApi.Domain.Entities;

namespace MoviesAPI.Dtos
{
    public class MovieDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        internal Maybe<Movie> Parse(Guid id)
        {
            if (string.IsNullOrEmpty(Name))
                return Maybe<Movie>.Nothing;
            if (string.IsNullOrEmpty(Description))
                return Maybe<Movie>.Nothing;

            return new Movie(id, Name, Description).ToMaybe();
        }
    }
}
