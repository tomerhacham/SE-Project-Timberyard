using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.API.Controllers.Models;
using WebService.API.Swagger.Example.QueriesController;
using WebService.API.Swagger.Example.WeatherForecastController;
using WebService.Domain.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Queries : ControllerBase
    {
        SystemFacade SystemInterface { get; }

        public Queries(SystemFacade systemInterface)
        {
            SystemInterface = systemInterface;
        }

        [Route("CardYield")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(CardYieldRequestExample))]
        [ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> CardYield([FromBody] CardYieldModel model)
        {
            var response = await SystemInterface.CalculateCardYield(model.StartDate, model.EndDate, model.Catalog);
            if (response.Status)
            {
                return Ok(response.Data);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

    }
}
