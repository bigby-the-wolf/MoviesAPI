using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using MoviesApi.Domain.Commands;
using MoviesApi.Domain.Entities;
using MoviesApi.Domain.Queries;
using MoviesAPI.Controllers;
using MoviesAPI.CQS;
using MoviesAPI.Dtos;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoviesApi.ApiTests.Controllers
{
    [TestFixture]
    public class MoviesControllerTests
    {
        [Test]
        public void PostRejectsNullInput()
        {
            var sut = GetSimpleSUT();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Post(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public async Task GetAllReturnsDtos()
        {
            var sut = GetSimpleSUT();

            var getAllResult = await sut.GetAll() as OkObjectResult;
            
            var movieDtos = getAllResult?.Value as IEnumerable<MovieDto>;
            Assert.IsNotNull(movieDtos);
        }

        [Test]
        public async Task GetReturnsDto()
        {
            var createMovieCommandHandlerAsync = Mock.Of<ICommandHandlerAsync<CreateMovieCommand>>();
            var getAllMoviesQueryHandlerAsync = Mock.Of<IQueryHandlerAsync<GetAllMoviesQuery, IReadOnlyCollection<Movie>>>();
            var movieId = Guid.NewGuid();
            var movie = new Movie(movieId, "Spider Man", "Our favorite super hero.");
            var getMovieByIdQueryHandlerAsync = Mock.Of<IQueryHandlerAsync<GetMovieByIdQuery, Maybe<Movie>>>(
                m => m.HandleAsync(It.Is<GetMovieByIdQuery>(q => q.Id == movieId)) == Task.FromResult(Maybe.From(movie)));
            var sut = new MoviesController(createMovieCommandHandlerAsync, getAllMoviesQueryHandlerAsync, getMovieByIdQueryHandlerAsync);

            var getResult = await sut.Get(movieId) as OkObjectResult;
            
            var movieDto = getResult?.Value as MovieDto;
            Assert.IsNotNull(movieDto);
        }

        [Test]
        public async Task GetReturnsNotFound()
        {
            var createMovieCommandHandlerAsync = Mock.Of<ICommandHandlerAsync<CreateMovieCommand>>();
            var getAllMoviesQueryHandlerAsync = Mock.Of<IQueryHandlerAsync<GetAllMoviesQuery, IReadOnlyCollection<Movie>>>();
            var movieId = Guid.NewGuid();
            var getMovieByIdQueryHandlerAsync = Mock.Of<IQueryHandlerAsync<GetMovieByIdQuery, Maybe<Movie>>>(
                m => m.HandleAsync(It.Is<GetMovieByIdQuery>(q => q.Id == movieId)) == Task.FromResult(Maybe<Movie>.None));
            var sut = new MoviesController(createMovieCommandHandlerAsync, getAllMoviesQueryHandlerAsync, getMovieByIdQueryHandlerAsync);

            var getResult = await sut.Get(movieId) as NotFoundResult;
            
            Assert.IsNotNull(getResult);
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

        [Test]
        public async Task GetRespondsOk()
        {
            var response = await GetMovie(Guid.NewGuid());
            
            Assert.True(
                response.IsSuccessStatusCode,
                $"Actual status code: {response.StatusCode}.");
        }

        private static MoviesController GetSimpleSUT()
        {
            var createMovieCommandHandlerAsync = Mock.Of<ICommandHandlerAsync<CreateMovieCommand>>();

            var movie = new Movie(Guid.NewGuid(), "Spider Man", "Our favorite super hero.");
            var getAllMoviesQueryHandlerAsync = Mock.Of<IQueryHandlerAsync<GetAllMoviesQuery, IReadOnlyCollection<Movie>>>(
                m => m.HandleAsync(It.IsAny<GetAllMoviesQuery>()) == Task.FromResult(new List<Movie> { movie } as IReadOnlyCollection<Movie>));
            var getMovieByIdQueryHandlerAsync = Mock.Of<IQueryHandlerAsync<GetMovieByIdQuery, Maybe<Movie>>>(
                m => m.HandleAsync(It.IsAny<GetMovieByIdQuery>()) == Task.FromResult(Maybe.From(movie)));

            return new MoviesController(createMovieCommandHandlerAsync, getAllMoviesQueryHandlerAsync, getMovieByIdQueryHandlerAsync);
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

        private static async Task<HttpResponseMessage> GetMovie(Guid id)
        {
            using var factory = new MovieApiFactory();
            var client = factory.CreateClient();

            return await client.GetAsync($"movies/{id}");
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

                    services.RemoveAll<IQueryHandlerAsync<GetAllMoviesQuery, IReadOnlyCollection<Movie>>>();
                    services.AddSingleton<IQueryHandlerAsync<GetAllMoviesQuery, IReadOnlyCollection<Movie>>>(new NullGetAllMoviesQueryHandlerAsync());

                    services.RemoveAll<IQueryHandlerAsync<GetMovieByIdQuery, Maybe<Movie>>>();
                    services.AddSingleton<IQueryHandlerAsync<GetMovieByIdQuery, Maybe<Movie>>>(new NullGetMovieByIdQueryHandlerAsync());
                });
            }

            private class NullCreateMovieCommandHandlerAsync : ICommandHandlerAsync<CreateMovieCommand>
            {
                public Task HandleAsync(CreateMovieCommand command)
                {
                    return Task.CompletedTask;
                }
            }

            private class NullGetAllMoviesQueryHandlerAsync : IQueryHandlerAsync<GetAllMoviesQuery, IReadOnlyCollection<Movie>>
            {
                public async Task<IReadOnlyCollection<Movie>> HandleAsync(GetAllMoviesQuery query)
                {
                    return await Task.Run(() => new List<Movie>());
                }
            }

            private class NullGetMovieByIdQueryHandlerAsync : IQueryHandlerAsync<GetMovieByIdQuery, Maybe<Movie>>
            {
                public async Task<Maybe<Movie>> HandleAsync(GetMovieByIdQuery query)
                {
                    var movie = new Movie(Guid.NewGuid(), "Spider Man", "Our favorite super hero.");
                    return await Task.Run(() => Maybe.From(movie));
                }
            }
        }
    }
}
