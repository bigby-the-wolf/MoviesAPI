using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using MoviesApi.Domain.Commands;
using MoviesAPI.Controllers;
using MoviesAPI.CQS;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoviesApi.ApiTests.Controllers
{
    [TestFixture]
    public class MoviesControllerTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private MoviesController _sut;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SetUp]
        public void Setup()
        {
            var createMovieCommandHandlerAsync = Mock.Of<ICommandHandlerAsync<CreateMovieCommand>>();

            _sut = new MoviesController(createMovieCommandHandlerAsync);
        }

        [Test]
        public void PostRejectsNullInput()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Post(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [TestCase(null, null)]
        [TestCase("Spider Man", null)]
        [TestCase(null, "Our favorite super hero.")]
        [TestCase("", "")]
        [TestCase("Spider Man", "")]
        [TestCase("", "Our favorite super hero.")]
        public async Task PostInvalidInput(string? name, string? description)
        {
            var response = await PostMovie(new {
                name,
                description
                });

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task PostValidMovie()
        {
            var response = await PostMovie(new {
                name = "Spider Man",
                description = "Our favorite super hero.",
                });
            
            Assert.True(
                response.IsSuccessStatusCode,
                $"Actual status code: {response.StatusCode}.");

            var contentJson = await response.Content.ReadAsStringAsync();
            var contentString = JsonSerializer.Deserialize<string>(contentJson);
            var couldParse = Guid.TryParse(contentString, out _);

            Assert.True(couldParse, $"Could not parse {contentString} to GUID.");
        }

        [Test]
        public async Task PostValidMovieWithId()
        {
            var movieDto = new {
                Id = Guid.NewGuid(),
                Name = "Spider Man",
                Description = "Our favorite super hero.",
                };

            var response = await PostMovie(movieDto);
            
            Assert.True(
                response.IsSuccessStatusCode,
                $"Actual status code: {response.StatusCode}.");

            var contentJson = await response.Content.ReadAsStringAsync();
            var contentString = JsonSerializer.Deserialize<string>(contentJson);
            var couldParse = Guid.TryParse(contentString, out Guid guid);

            Assert.True(couldParse, $"Could not parse {contentString} to GUID.");
            Assert.AreEqual(movieDto.Id , guid, "Must preserve incoming GUID.");
        }

        [Test]
        public async Task GetAllRespondsOk()
        {
            var response = await GetAllMovies();
            
            Assert.True(
                response.IsSuccessStatusCode,
                $"Actual status code: {response.StatusCode}.");
        }

        private static async Task<HttpResponseMessage> PostMovie(object movie)
        {
            using var factory = new MovieApiFactory();
            var client = factory.CreateClient();

            string json = JsonSerializer.Serialize(movie);
            using var content = new StringContent(json);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            content.Headers.ContentType.MediaType = "application/json";
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return await client.PostAsync("movies", content);
        }

        private static async Task<HttpResponseMessage> GetAllMovies()
        {
            using var factory = new MovieApiFactory();
            var client = factory.CreateClient();

            return await client.GetAsync("movies");
        }

        private class MovieApiFactory : WebApplicationFactory<MoviesController>
        {
            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                if (builder is null)
                    throw new ArgumentNullException(nameof(builder));
        
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<ICommandHandlerAsync<CreateMovieCommand>>();
                    services.AddSingleton<ICommandHandlerAsync<CreateMovieCommand>>(new NullCreateMovieCommandHandlerAsync());
                });
            }

            private class NullCreateMovieCommandHandlerAsync : ICommandHandlerAsync<CreateMovieCommand>
            {
                public Task HandleAsync(CreateMovieCommand command)
                {
                    return Task.CompletedTask;
                }
            }
        }
    }
}
