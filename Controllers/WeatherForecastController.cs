// The dynamo DB code was from this tutotial https://www.youtube.com/watch?v=BbUmLRaxZG8
// The Secrets Manager stuff was from https://www.youtube.com/watch?v=wIuP2RKy4z4

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.SecretsManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace dynamodb_charp_lambda.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IDynamoDBContext _dynamoDBContext;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(
            IDynamoDBContext dynamoDBContext,
            ILogger<WeatherForecastController> logger)
        {
            _dynamoDBContext = dynamoDBContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get(string city = "Leeds")
        {
            return await _dynamoDBContext.
                QueryAsync<WeatherForecast>(city, Amazon.DynamoDBv2.DocumentModel.QueryOperator.Between, new object[] {DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(2) })
                .GetRemainingAsync();

            //return GenerateDummyWeatherForecast(city);
        }

        [HttpPost]
        public async Task Post (string city)
        {
            var data = GenerateDummyWeatherForecast(city);
            foreach (var item in data)
            {
                await _dynamoDBContext.SaveAsync(item);
            }

            //var specificItem = await _dynamoDBContext.LoadAsync<WeatherForecast>(city, DateTime.Now.Date.AddDays(1));
            // specificItem.Summary = "Test summary";
            // await _dynamoDBContext.SaveAsync(specificItem);
        }

        [HttpDelete]
        public async Task Delete(string city)
        {
            var specificItem = await _dynamoDBContext.LoadAsync<WeatherForecast>(city, DateTime.Now.Date.AddDays(1));
            await _dynamoDBContext.DeleteAsync(specificItem);
        }

        private static IEnumerable<WeatherForecast> GenerateDummyWeatherForecast(string city)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                City = city,
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}