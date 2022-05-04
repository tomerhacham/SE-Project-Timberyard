using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Threading.Tasks;
using WebService.API.Controllers.Models;
using WebService.API.Swagger.Example.AlarmsController;
using WebService.API.Swagger.Example.AuthenticationController;
using WebService.Domain.Interface;
using WebService.Utils;

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

        [Route("Login")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(LoginExample))]
        //[ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            Result<JWTtoken> result = await SystemInterface.Login(model.Email, model.Password);
            if (result.Status)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [Route("AddUser")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(AddUserExample))]
        //[ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> AddUser([FromBody] UserCRUDModel model)
        {
            Result<bool> result = await SystemInterface.AddUser(model.Email);
            if (result.Status)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [Route("RemoveUser")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(RemoveUserExample))]
        //[ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> RemoveUser([FromBody] UserCRUDModel model)
        {
            Result<bool> result = await SystemInterface.RemoveUser(model.Email);
            if (result.Status)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [Route("ChangeSystemAdminPassword")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(ChangeSystemAdminPasswordExample))]
        //[ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> ChangeSystemAdminPassword([FromBody] ChangeSystemAdminPasswordModel model)
        {
            Result<bool> result = await SystemInterface.ChangeSystemAdminPassword(model.Email, model.NewPassword, model.OldPassword);
            if (result.Status)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [Route("AddSystemAdmin")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(AddSystemAdminExample))]
        //[ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> AddSystemAdmin([FromBody] SystemAdminModel model)
        {
            Result<bool> result = await SystemInterface.AddSystemAdmin(model.SystemAdminEmail);
            if (result.Status)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [Route("ForgetPassword")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(ForgetPasswordExample))]
        //[ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> RemoveSystemAdmin([FromBody] ForgetPasswordModel model)
        {
            Result<bool> result = await SystemInterface.ForgetPassword(model.Email);
            if (result.Status)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

    }
}
