using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebService.API.Controllers.Models;
using WebService.API.Swagger.Example.AlarmsController;
using WebService.Domain.DataAccess.DTO;
using WebService.Domain.Interface;

namespace WebService.API.Controllers
{
    /// <summary>
    /// Responsible on all alarms management scenarios
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class Alarms : ControllerBase
    {
        SystemFacade SystemInterface { get; }

        public Alarms(SystemFacade systemInterface)
        {
            SystemInterface = systemInterface;
        }
        /// <summary>
        /// Adding new alarm to the system
        /// </summary>
        /// <param name="model"></param>
        /// <returns>new alarm object</returns>
        [Route("AddNewAlarm")]
        [HttpPost]
        [Authorize(Role.Admin)]
        [SwaggerRequestExample(typeof(object), typeof(AddNewAlarmRequestExample))]
        [ProducesResponseType(typeof(AddNewAlarmResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddNewAlarmResponseExample))]
        public async Task<IActionResult> AddNewAlarm([FromBody] PartialAlarmModel model)
        {

            var response = await SystemInterface.AddNewAlarm(model.Name, model.Field, model.Objective, model.Threshold, model.Receivers);
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
        /// Editing existing alarm
        /// </summary>
        /// <param name="model"></param>
        /// <returns>updated alarm object</returns>
        [Route("EditAlarm")]
        [HttpPost]
        [Authorize(Role.Admin)]
        [SwaggerRequestExample(typeof(object), typeof(FullAlarmRequestExample))]
        [ProducesResponseType(typeof(FullAlarmRequestExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FullAlarmRequestExample))]
        public async Task<IActionResult> EditAlarm([FromBody] FullAlarmModel model)
        {

            var response = await SystemInterface.EditAlarm(model.Id, model.Name, model.Field, model.Objective, model.Threshold, model.Active, model.Receivers);
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
        /// Removing alarm from the system
        /// </summary>
        /// <param name="Id">Id of th alarm to remove</param>
        /// <returns></returns>
        [Route("RemoveAlarm")]
        [HttpPost]
        [Authorize(Role.Admin)]
        [SwaggerRequestExample(typeof(object), typeof(AlarmToRemoveRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OkResult))]
        public async Task<IActionResult> RemoveAlarm([FromBody] AlarmToRemoveModel model)
        {

            var response = await SystemInterface.RemoveAlarm(model.Id);
            if (response.Status)
            {
                return Ok();
            }
            else
            {
                var error = new ErrorsModel() { Errors = new List<string>() { response.Message } };
                return BadRequest(error);
            }
        }
        /// <summary>
        /// Get all alarms object listed in the system
        /// This function should be use only to display in the UI all the alarms
        /// </summary>
        /// <returns>List of alarms</returns>
        [Route("GetAllAlarms")]
        [HttpPost]
        [Authorize(Role.Admin)]
        [ProducesResponseType(typeof(GetAllAlarmsResponseExample), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetAllAlarmsResponseExample))]
        public async Task<IActionResult> GetAllAlarms()
        {

            var response = await SystemInterface.GetAllAlarms();
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
        /// End point which whill use only by the ETL process to signal that new batch of logs has been inserted
        /// </summary>
        /// <returns></returns>
        [Route("CheckAlarmsCondition")]
        [HttpPost]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OkResult))]
        public async Task<IActionResult> CheckAlarmsCondition()
        {

            await SystemInterface.CheckAlarmsCondition();
            return Ok();

        }
    }
}
