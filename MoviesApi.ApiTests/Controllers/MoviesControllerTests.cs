using Microsoft.AspNetCore.Mvc.Testing;
using MoviesAPI.Controllers;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoviesApi.ApiTests.Controllers
{
    public class MoviesControllerTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private MoviesController _sut;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SetUp]
        public void Setup()
        {
            _sut = new MoviesController();
        }

        [Test]
        public void PostRejectsNullInput()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Post(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
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
            var guid = Guid.TryParse(contentString, out _);

            Assert.True(guid, $"Could not parse {contentString} to GUID.");
        }

        private static async Task<HttpResponseMessage> PostMovie(object movie)
        {
            using var factory = new WebApplicationFactory<MoviesController>();
            var client = factory.CreateClient();

            string json = JsonSerializer.Serialize(movie);
            using var content = new StringContent(json);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            content.Headers.ContentType.MediaType = "application/json";
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return await client.PostAsync("movies", content);
        }
    }
}
