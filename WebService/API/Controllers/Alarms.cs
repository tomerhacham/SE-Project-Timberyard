using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebService.API.Controllers.Models;
using WebService.API.Swagger.Example.QueriesController;
using WebService.Domain.Interface;

namespace WebService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Alarms : ControllerBase
    {
        SystemFacade SystemInterface { get; }

        public Alarms(SystemFacade systemInterface)
        {
            SystemInterface = systemInterface;
        }

        [Route("AddNewAlarm")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(CardYieldRequestExample))]
        [ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> AddNewAlarm([FromBody] AlarmModel model)
        {

            var response = await SystemInterface.AddNewAlarm(model.Name,model.Field,model.Threshold,model.Receivers);
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

    }
}
