using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebService.API.Controllers.Models;
using WebService.API.Swagger.Example.AlarmsController;
using WebService.API.Swagger.Example.AuthenticationController;
using WebService.Domain.DataAccess.DTO;
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
        public async Task<IActionResult> RequestVerificationCode([FromBody] EmailModel model)
        {
            await SystemInterface.RequestVerificationCode(model.Email);
            // always return OK due to security issues
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// if user logged in sccessfuly, return JWT token. else, return empty string  
        /// </returns>
        [Route("Login")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(LoginExample))]
        //[ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            Result<JWTtoken> result = await SystemInterface.Login(model.Email, model.Password);
            return Ok(result.Data);
        }

        [Route("AddUser")]
        [Authorize(Role.Admin)]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(AddUserExample))]
        //[ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> AddUser([FromBody] EmailModel model)
        {
            Result<bool> result = await SystemInterface.AddUser(model.Email);
            return Ok(result.Status);
        }

        [Route("RemoveUser")]
        [Authorize(Role.Admin)]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(RemoveUserExample))]
        //[ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> RemoveUser([FromBody] EmailModel model)
        {
            Result<bool> result = await SystemInterface.RemoveUser(model.Email);
            return Ok(result.Status);
        }

        [Route("ChangeSystemAdminPassword")]
        [Authorize(Role.Admin)]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(ChangeSystemAdminPasswordExample))]
        //[ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> ChangeSystemAdminPassword([FromBody] ChangeSystemAdminPasswordModel model)
        {
            Result<bool> result = await SystemInterface.ChangeSystemAdminPassword((string)HttpContext.Items["Email"], model.NewPassword, model.OldPassword);
            return Ok(result.Status);
        }

        [Route("AddSystemAdmin")]
        [Authorize(Role.Admin)]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(AddSystemAdminExample))]
        //[ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> AddSystemAdmin([FromBody] EmailModel model)
        {
            Result<bool> result = await SystemInterface.AddSystemAdmin(model.Email);
            return Ok(result.Status);
        }

        [Route("ForgetPassword")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(ForgetPasswordExample))]
        //[ProducesResponseType(typeof(CardYieldResponseExample), StatusCodes.Status200OK)]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(CardYieldResponseExample))]
        public async Task<IActionResult> ForgetPassword([FromBody] EmailModel model)
        {
            Result<bool> result = await SystemInterface.ForgetPassword(model.Email);
            return Ok(result.Status);
        }

        [Route("getToken")]
        [HttpGet]
        public async Task<IActionResult> GetToken([Required] string email)
        {
            return Ok(await SystemInterface.GetToken(email));
        }

        [Route("GetAllUsers")]
        [Authorize(Role.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            Result<List<UserDTO>> result = await SystemInterface.GetAllUsers();
            return Ok(result.Data);
        }

    }
}
