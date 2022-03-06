using Microsoft.EntityFrameworkCore;
using MoviesApi.Domain.Queries;
using MoviesApi.EntityFramework.QueryHandlers;
using MoviesApi.EntityFramework.Tests.Fixtures;
using MoviesApi.EntityFramework.Tests.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MoviesApi.EntityFramework.Tests.QueryHandlers
{
    public class GetMovieByIdQueryHandlerAsyncTests  : IClassFixture<TestDatabaseFixture>
    {
        public GetMovieByIdQueryHandlerAsyncTests(TestDatabaseFixture dbFixture)
            => DbFixture = dbFixture;

        public TestDatabaseFixture DbFixture { get; }

        [Fact]
        public async Task HandleGetMovieById()
        {
            using var context = DbFixture.CreateContext();
            var sut = new GetMovieByIdQueryHandlerAsync(context);
            var movieId = new Guid("15C02618-C2B4-4443-A92C-8EA3115F2F57");
            var getAllMoviesQuery = new GetMovieByIdQuery(movieId);

            var movie = await sut.HandleAsync(getAllMoviesQuery);

            Assert.True(movie.HasValue, "Could not find movie by id.");
            var movieInDb = await context.Movies.AsNoTracking().Where(m => m.Id == movieId).SingleOrDefaultAsync();
            Assert.Equal(movieInDb!, movie.Value, new MovieEqualityComparer());
        }

        [Fact]
        public async Task HandleMovieNotFoundById()
        {
            using var context = DbFixture.CreateContext();
            var sut = new GetMovieByIdQueryHandlerAsync(context);
            var getAllMoviesQuery = new GetMovieByIdQuery(Guid.NewGuid());

            var movie = await sut.HandleAsync(getAllMoviesQuery);

            Assert.True(movie.HasNoValue, "Movie should not be in DB.");
        }
    }
}
