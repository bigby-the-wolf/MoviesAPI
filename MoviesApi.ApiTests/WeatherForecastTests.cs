using MoviesAPI;
using NUnit.Framework;
using System;

namespace MoviesApi.ApiTests
{
    public class WeatherForecastTests
    {
        private static int FromCelciusToFarenheit(int temperatureC)
        {
            return 32 + (int)(temperatureC / 0.5556);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TemeperatureF_Gets_Converted_Correctly()
        {
            var weatherForecast = new WeatherForecast { Date = DateTime.Now, TemperatureC = 32, Summary = "Hot" };

            var temperatureF = FromCelciusToFarenheit(weatherForecast.TemperatureC);

            Assert.AreEqual(temperatureF, weatherForecast.TemperatureF);
        }
    }
}
