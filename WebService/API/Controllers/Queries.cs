using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebService.API.Controllers.Models;
using WebService.API.Swagger.Example.QueriesController;
using WebService.Domain.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebService.API.Controllers
{
    /// <summary>
    /// Responsible on all query calculation scenarios
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class Queries : ControllerBase
    {
        SystemFacade SystemInterface { get; }

        public Queries(SystemFacade systemInterface)
        {
            SystemInterface = systemInterface;
        }

        /// <summary>
        /// Calcuate card yield by catalog and dates range
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CardYield")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(CardYieldRequestExample))]
        [ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> CardYield([FromBody] CatalogWithDatesModel model)
        {

            var response = await SystemInterface.CalculateCardYield(model.Catalog, model.StartDate, model.EndDate);
            if (response.Status)
            {
                return Ok(response.Data);
            }
            else
            {
                var error = new ErrorsModel() { Errors = new List<string>() { response.Message } };
                return BadRequest(error);
            }
        }

        /// <summary>
        /// Calculate Station yield for all station by dates range
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("StationsYield")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(StationsYieldRequestExample))]
        [ProducesResponseType(typeof(StationsYieldResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StationsYieldResponseExample))]
        public async Task<IActionResult> StationsYield([FromBody] DatesModel model)
        {

            var response = await SystemInterface.CalculateStationsYield(model.StartDate, model.EndDate);
            if (response.Status)
            {
                return Ok(response.Data);
            }
            else
            {
                var error = new ErrorsModel() { Errors = new List<string>() { response.Message } };
                return BadRequest(error);
            }
        }

        /// <summary>
        /// Calculate station and card yield by catalog, station and date of range
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("StationAndCardYield")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(StationAndCardYieldRequestExample))]
        [ProducesResponseType(typeof(StationAndCardYieldResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StationAndCardYieldResponseExample))]
        public async Task<IActionResult> StationAndCardYield([FromBody] CatalogStationWithDatesModel model)
        {
            var response = await SystemInterface.CalculateStationAndCardYield(model.Station, model.Catalog, model.StartDate, model.EndDate);
            if (response.Status)
            {
                return Ok(response.Data);
            }
            else
            {
                var error = new ErrorsModel() { Errors = new List<string>() { response.Message } };
                return BadRequest(error);
            }
        }

        /// <summary>
        /// Calcuate no failure found details by card name, dates range and time interval between to runs on the same serial number [in seconds]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("NFF")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(NFFRequestExample))]
        [ProducesResponseType(typeof(NFFResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(NFFResponseExample))]
        public async Task<IActionResult> NoFailureFound([FromBody] CardNameDatesTimeintervalModel model)
        {

            var response = await SystemInterface.CalculateNFF(model.CardName, model.StartDate, model.EndDate, model.TimeInterval);
            if (response.Status)
            {
                return Ok(response.Data);
            }
            else
            {
                var error = new ErrorsModel() { Errors = new List<string>() { response.Message } };
                return BadRequest(error);
            }
        }

        /// <summary>
        /// Calculate load metric for all the testers in dates range
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("TesterLoad")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(TesterLoadRequestExample))]
        [ProducesResponseType(typeof(TesterLoadResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TesterLoadResponseExample))]
        public async Task<IActionResult> TesterLoad([FromBody] DatesModel model)
        {
            var response = await SystemInterface.CalculateTesterLoad(model.StartDate, model.EndDate);
            if (response.Status)
            {
                return Ok(response.Data);
            }
            else
            {
                var error = new ErrorsModel() { Errors = new List<string>() { response.Message } };
                return BadRequest(error);
            }
        }

        /// <summary>
        /// Calculate test duration by catalog and dates range
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CardTestDuration")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(CardTestDurationRequestExample))]
        [ProducesResponseType(typeof(CardTestDurationResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardTestDurationResponseExample))]
        public async Task<IActionResult> CardTestDuration([FromBody] CatalogWithDatesModel model)
        {
            var response = await SystemInterface.CalculateCardTestDuration(model.Catalog, model.StartDate, model.EndDate);
            if (response.Status)
            {
                return Ok(response.Data);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        /// <summary>
        /// Calculate boundaries metrics (Min,Max,Avg,Std..) by catalog and dates range
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Boundaries")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(BoundariesRequestExample))]
        [ProducesResponseType(typeof(BoundariesResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BoundariesResponseExample))]
        public async Task<IActionResult> Boundaries([FromBody] CatalogWithDatesModel model)
        {
            var response = await SystemInterface.CalculateBoundaries(model.Catalog, model.StartDate, model.EndDate);
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
