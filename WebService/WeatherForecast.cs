using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Threading.Tasks;
using WebService.API.Swagger.Example.WeatherForecastController;

namespace WebService
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }

        [Route("SimpleGet")]
        [HttpGet]
        [SwaggerRequestExample(typeof(object), typeof(ToDoRequestExample))]
        [ProducesResponseType(typeof(ToDoResponseExample), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ToDoBadResponseExample), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ToDoResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ToDoBadResponseExample))]
        public async Task<object> SimpleGet(object model)
        {
            return new { Attribute = "simpleGet" };
        }
    }
}