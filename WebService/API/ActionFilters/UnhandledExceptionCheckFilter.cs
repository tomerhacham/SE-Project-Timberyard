using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using WebService.API.Controllers.Models;
using WebService.Utils;

namespace WebService.API.ActionFilters
{
    public class UnhandledExceptionCheckFilter : IActionFilter
    {

        private readonly ILogger _logger;

        public UnhandledExceptionCheckFilter(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Method been called after execution of the action has been completed
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null)
            {
                return;
            }
            var ErrorMessage = context.Exception.Message;
            var errors = new ErrorsModel() { Errors = new List<string>() { ErrorMessage } };
            context.Result = new BadRequestObjectResult(new { Errors = errors });
            context.ExceptionHandled = true;
            _logger.Warning($"Unhandled exception", context.Exception);

        }

        /// <summary>
        /// Method been called in the begining of the execution of the action
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context) { }
    }
}
