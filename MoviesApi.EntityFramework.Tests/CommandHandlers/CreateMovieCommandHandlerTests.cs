using AutoFixture;
using MoviesApi.Domain.Commands;
using MoviesApi.Domain.Entities;
using MoviesApi.EntityFramework.CommandHandlers;
using MoviesApi.EntityFramework.Tests.Fixtures;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MoviesApi.EntityFramework.Tests.CommandHandlers
{
    public class CreateMovieCommandHandlerTests : IClassFixture<TestDatabaseFixture>
    {
        public CreateMovieCommandHandlerTests(TestDatabaseFixture dbFixture)
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
            var movieComparer = new MovieEqualityComparer();
            Assert.True(movieComparer.Equals(createMovieCommand.Movie, movie));
        }

        private class MovieEqualityComparer : IEqualityComparer<Movie>
        {
            public bool Equals(Movie? x, Movie? y)
            {
                if (x == null && y == null)
                    return true;
                else if (x == null || y == null)
                    return false;
                
                return x.Id == y.Id
                    && x.Name == y.Name
                    && x.Description == y.Description;
            }

            public int GetHashCode([DisallowNull] Movie obj)
            {
                return obj.Id.GetHashCode() ^ obj.Name.GetHashCode() ^ obj.Description.GetHashCode();
            }
        }
    }
}
