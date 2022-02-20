using Microsoft.EntityFrameworkCore;
using MoviesApi.Domain.Entities;

namespace MoviesApi.EntityFramework
{
    public class MoviesContext : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MoviesContext(DbContextOptions<MoviesContext> options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            : base(options)
        { }

        public DbSet<Movie> Movies { get; set; }
    }
}
