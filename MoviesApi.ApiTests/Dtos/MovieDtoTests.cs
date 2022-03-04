using MoviesAPI.Dtos;
using NUnit.Framework;
using System;

namespace MoviesApi.ApiTests.Dtos
{
    [TestFixture]
    public class MovieDtoTests
    {
        [TestCase(null, null)]
        [TestCase("Spider Man", null)]
        [TestCase(null, "Our favorite super hero.")]
        [TestCase("", "")]
        [TestCase("Spider Man", "")]
        [TestCase("", "Our favorite super hero.")]
        public void ParseInvalidInput(string name, string description)
        {
            var movieDto = new MovieDto { Name = name, Description = description };
            
            var movie = movieDto.Parse();

            Assert.False(movie.HasValue, "Parsing the dto requires non empty name and description.");
        }

        [Test]
        public void ParseValidInput()
        {
            var movieDto = new MovieDto { Name = "Spider Man", Description = "Our favorite super hero." };

            var movie = movieDto.Parse();

            Assert.True(movie.HasValue, "Could not parse the dto.");
            Assert.AreEqual(movieDto.Name, movie.Value.Name);
            Assert.AreEqual(movieDto.Description, movie.Value.Description);
        }

        [Test]
        public void ParseProvidesIdIfNullOrEmpty()
        {
            var movieDto = new MovieDto { Name = "Spider Man", Description = "Our favorite super hero." };
            
            var movie = movieDto.Parse();
            
            Assert.True(movie.HasValue, "Could not parse the dto.");
            Assert.NotNull(movie.Value.Id);
            Assert.AreNotEqual(Guid.Empty, movie.Value.Id);
        }

        [Test]
        public void ParsePreservesId()
        {
            var movieDto = new MovieDto { Id = Guid.NewGuid(), Name = "Spider Man", Description = "Our favorite super hero." };
            
            var movie = movieDto.Parse();
            
            Assert.True(movie.HasValue, "Could not parse the dto.");
            Assert.NotNull(movie.Value.Id);
            Assert.AreEqual(movieDto.Id, movie.Value.Id);
        }
    }
}
