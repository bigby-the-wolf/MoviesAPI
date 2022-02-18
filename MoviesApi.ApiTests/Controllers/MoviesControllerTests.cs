using Microsoft.AspNetCore.Mvc.Testing;
using MoviesAPI.Controllers;
using NUnit.Framework;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoviesApi.ApiTests.Controllers
{
    public class MoviesControllerTests
    {
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
