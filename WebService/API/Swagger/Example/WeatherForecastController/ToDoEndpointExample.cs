using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.API.Swagger.Example.WeatherForecastController
{
    /// <summary>
    /// Example for valid request from client to API
    /// </summary>
    public class ToDoRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new { Attribute = "This is request example" };
        }
    }
    /// <summary>
    /// Example for valid response from the API to the client
    /// </summary>
    public class ToDoResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new { Attribute = "This is good response" };
        }
    }

    public class ToDoBadResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new {Attribute ="This is bad response" };
        }
    }
}
