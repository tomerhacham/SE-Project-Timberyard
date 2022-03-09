using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Threading.Tasks;
using WebService.API.Swagger.Example.WeatherForecastController;
using WebService.Domain.Interface;

namespace WebService
{
    [Route("api/[controller]")]
    [ApiController]
    public class Example
    {
        SystemFacade SystemInterface { get; }

        public Example(SystemFacade systemInterface)
        {
            SystemInterface = systemInterface;
        }

        [Route("SimplePost")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(ToDoRequestExample))]
        [ProducesResponseType(typeof(ToDoResponseExample), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ToDoBadResponseExample), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ToDoResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ToDoBadResponseExample))]
        public async Task<object> SimplePost(object model)
        {
            return await SystemInterface.QueriesController.LogsAndTestsRepository.DynamicReturnTypeExampleQuery();
            return new { Attribute = "simpleGet" };
        }
    }
}
