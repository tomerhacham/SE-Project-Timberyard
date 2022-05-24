using Microsoft.AspNetCore.Http;
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
    /// <summary>
    /// Responsible on all authentication and users management scenarios
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class Authentication : ControllerBase
    {
        SystemFacade SystemInterface { get; }

        public Authentication(SystemFacade systemInterface)
        {
            SystemInterface = systemInterface;
        }
        /// <summary>
        /// Request verification code to be sent to the provided email.
        /// This function should be use on regular user loging flow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("RequestVerificationCode")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(RequestVerificationCodeExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OkResult))]
        public async Task<IActionResult> RequestVerificationCode([FromBody] EmailModel model)
        {
            await SystemInterface.RequestVerificationCode(model.Email);
            // always return OK due to security issues
            return Ok();
        }

        /// <summary>
        /// Perform login and generated token for the user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// if user logged in sccessfuly, return JWT token. else, return empty string  
        /// </returns>
        [Route("Login")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(LoginRequestExample))]
        [ProducesResponseType(typeof(LoginResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(LoginResponseExample))]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            Result<JWTtoken> result = await SystemInterface.Login(model.Email, model.Password);
            return Ok(result.Data);
        }

        /// <summary>
        /// Adding new email as regular user to the system
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddUser")]
        [Authorize(Role.Admin)]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(AddUserExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OkResult))]
        public async Task<IActionResult> AddUser([FromBody] EmailModel model)
        {
            Result<bool> result = await SystemInterface.AddUser(model.Email);
            return Ok(result.Status);
        }

        /// <summary>
        /// Remove user (system admin or regular user) from the system
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("RemoveUser")]
        [Authorize(Role.Admin)]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(RemoveUserExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OkResult))]
        public async Task<IActionResult> RemoveUser([FromBody] EmailModel model)
        {
            Result<bool> result = await SystemInterface.RemoveUser(model.Email);
            return Ok(result.Status);
        }
        /// <summary>
        /// Change system admin password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ChangeSystemAdminPassword")]
        [Authorize(Role.Admin)]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(ChangeSystemAdminPasswordExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OkResult))]
        public async Task<IActionResult> ChangeSystemAdminPassword([FromBody] ChangeSystemAdminPasswordModel model)
        {
            Result<bool> result = await SystemInterface.ChangeSystemAdminPassword((string)HttpContext.Items["Email"], model.NewPassword, model.OldPassword);
            return Ok(result.Status);
        }
        /// <summary>
        /// Add new email as new system admin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddSystemAdmin")]
        [Authorize(Role.Admin)]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(AddSystemAdminExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OkResult))]
        public async Task<IActionResult> AddSystemAdmin([FromBody] EmailModel model)
        {
            Result<bool> result = await SystemInterface.AddSystemAdmin(model.Email);
            return Ok(result.Status);
        }
        /// <summary>
        /// Reset the provided user's password and send the new password via SMTP
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ForgetPassword")]
        [HttpPost]
        [SwaggerRequestExample(typeof(object), typeof(ForgetPasswordExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OkResult))]
        public async Task<IActionResult> ForgetPassword([FromBody] EmailModel model)
        {
            Result<bool> result = await SystemInterface.ForgetPassword(model.Email);
            return Ok(result.Status);
        }
        /// <summary>
        /// Util function - should be for test purposes only
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [Route("getToken")]
        [HttpGet]
        public async Task<IActionResult> GetToken([Required] string email)
        {
            return Ok(await SystemInterface.GetToken(email));
        }
        /// <summary>
        /// Get all users listed in the system
        /// This function should be use only to display in the UI all the users
        /// </summary>
        /// <returns>List of users</returns>
        [Route("GetAllUsers")]
        [Authorize(Role.Admin)]
        [ProducesResponseType(typeof(GetAllUsersResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetAllUsersResponseExample))]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            Result<List<UserDTO>> result = await SystemInterface.GetAllUsers();
            return Ok(result.Data);
        }

    }
}
