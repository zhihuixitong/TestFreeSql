using Db.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestFreeSql.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly IFreeSql _freeSql;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IFreeSql  freeSql)
        {
            _logger = logger;
            _freeSql = freeSql;
        }

        [HttpGet]
        public   IEnumerable<WeatherForecast> Get()
        {

           var save= _freeSql.Insert<Blog>(new Blog() { BlogId = 1, Rating = 1, Url = "123" }).ExecuteAffrows();


            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
