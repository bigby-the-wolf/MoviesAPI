using AutoFixture;
using MoviesApi.Domain.Commands;
using MoviesApi.EntityFramework.CommandHandlers;
using MoviesApi.EntityFramework.Tests.Fixtures;
using MoviesApi.EntityFramework.Tests.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MoviesApi.EntityFramework.Tests.CommandHandlers
{
    public class CreateMovieCommandHandlerAsyncTests : IClassFixture<TestDatabaseFixture>
    {
        public CreateMovieCommandHandlerAsyncTests(TestDatabaseFixture dbFixture)
            => DbFixture = dbFixture;

        public TestDatabaseFixture DbFixture { get; }

        [Fact]
        public async Task HandleCreateMovie()
        {
            using var context = DbFixture.CreateContext();
            var sut = new CreateMovieCommandHandlerAsync(context);
            var createMovieCommand = new Fixture().Create<CreateMovieCommand>();

            context.Database.BeginTransaction();

            await sut.HandleAsync(createMovieCommand);

            context.ChangeTracker.Clear();

            var movie = context.Movies.Single(m => m.Id == createMovieCommand.Movie.Id);
            Assert.Equal(createMovieCommand.Movie, movie, new MovieEqualityComparer());
        }
    }
}
