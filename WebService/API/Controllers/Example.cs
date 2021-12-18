using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Threading.Tasks;
using WebService.API.Swagger.Example.WeatherForecastController;

namespace WebService
{
    [Route("api/[controller]")]
    [ApiController]
    public class Example
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }

        [Route("SimplePost")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(ToDoRequestExample))]
        [ProducesResponseType(typeof(ToDoResponseExample), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ToDoBadResponseExample), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ToDoResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ToDoBadResponseExample))]
        public async Task<object> SimplePost(object model)
        {
            return new { Attribute = "simpleGet" };
        }
    }
}
