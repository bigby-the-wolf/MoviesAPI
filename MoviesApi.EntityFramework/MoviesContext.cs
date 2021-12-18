using Microsoft.EntityFrameworkCore;

namespace MoviesApi.EntityFramework
{
    public class MoviesContext : DbContext
    {
        public MoviesContext(DbContextOptions<MoviesContext> options)
            : base(options)
        { }
    }
}
