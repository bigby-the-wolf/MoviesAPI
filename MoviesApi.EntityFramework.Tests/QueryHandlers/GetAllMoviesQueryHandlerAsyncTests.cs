using Microsoft.EntityFrameworkCore;
using MoviesApi.Domain.Queries;
using MoviesApi.EntityFramework.QueryHandlers;
using MoviesApi.EntityFramework.Tests.Fixtures;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MoviesApi.EntityFramework.Tests.QueryHandlers
{
    public class GetAllMoviesQueryHandlerAsyncTests  : IClassFixture<TestDatabaseFixture>
    {
        public GetAllMoviesQueryHandlerAsyncTests(TestDatabaseFixture dbFixture)
            => DbFixture = dbFixture;

        public TestDatabaseFixture DbFixture { get; }

        [Fact]
        public async Task HandleGetAllMovies()
        {
            using var context = DbFixture.CreateContext();
            var sut = new GetAllMoviesQueryHandlerAsync(context);
            var getAllMoviesQuery = new GetAllMoviesQuery();

            var movies = await sut.HandleAsync(getAllMoviesQuery);

            var moviesInDb = await context.Movies.ToListAsync();
            Assert.False(moviesInDb.Except(movies).Any(), "Must get exactly all movies from DB.");
        }
    }
}
