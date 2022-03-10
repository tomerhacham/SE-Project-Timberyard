using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Threading.Tasks;
using WebService.API.Controllers.Models;
using WebService.API.Swagger.Example.QueriesController;
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
            if (ModelState.IsValid)
            {
                var response = await SystemInterface.CalculateCardYield(model.Catalog, model.StartDate, model.EndDate);
                if (response.Status)
                {
                    return Ok(response.Data);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            else
            {
                return BadRequest();
            }

        }

        [Route("StationsYield")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(StationsYieldRequestExample))]
        [ProducesResponseType(typeof(StationsYieldResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StationsYieldResponseExample))]
        public async Task<IActionResult> StationsYield([FromBody] StationsYieldModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await SystemInterface.CalculateStationsYield(model.StartDate, model.EndDate);
                if (response.Status)
                {
                    return Ok(response.Data);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            else
            {
                return BadRequest();
            }

        }

        [Route("StationAndCardYield")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(StationAndCardYieldRequestExample))]
        [ProducesResponseType(typeof(StationAndCardYieldResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StationAndCardYieldResponseExample))]
        public async Task<IActionResult> StationAndCardYield([FromBody] StationAndCardYieldModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await SystemInterface.CalculateStationAndCardYield(model.Station, model.Catalog, model.StartDate, model.EndDate);
                if (response.Status)
                {
                    return Ok(response.Data);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            else
            {
                return BadRequest();
            }

        }

        [Route("NFF")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(NFFRequestExample))]
        [ProducesResponseType(typeof(NFFResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(NFFResponseExample))]
        public async Task<IActionResult> NoFailureFound([FromBody] NoFailureFoundModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await SystemInterface.CalculateNFF(model.CardName, model.StartDate, model.EndDate);
                if (response.Status)
                {
                    return Ok(response.Data);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            else
            {
                return BadRequest();
            }

        }
    }
}
