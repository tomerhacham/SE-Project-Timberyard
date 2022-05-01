using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Threading.Tasks;
using WebService.API.Controllers.Models;
using WebService.API.Swagger.Example.AlarmsController;
using WebService.Domain.Interface;

namespace WebService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Authentication : ControllerBase
    {
        SystemFacade SystemInterface { get; }

        public Authentication(SystemFacade systemInterface)
        {
            SystemInterface = systemInterface;
        }

        [Route("RequestVerificationCode")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(RequestVerificationCodeExample))]
        //[ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> RequestVerificationCode([FromBody] RequestVerificationCodeModel model)
        {
            await SystemInterface.RequestVerificationCode(model.Email);
            // always return OK due to security issues
            return Ok();
        }

    }
}
