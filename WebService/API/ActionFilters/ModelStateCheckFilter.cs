using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using WebService.Utils;

namespace WebService.API.ActionFilters
{
    /// <summary>
    /// Middleware for verifing model state.
    /// Instead of verifing the model state explicilty the middleware will verigy it implicitly.
    /// Hence, not valid requests will be handeled by this middleware
    /// </summary>
    public sealed class ModelStateCheckFilter : IActionFilter
    {
        public readonly ILogger Logger;

        public ModelStateCheckFilter(ILogger logger)
        {
            Logger = logger;
        }


        /// <summary>
        /// Method been called after exection of the action has been completed
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //Logger.Info(context.Result?.ToString());
        }

        /// <summary>
        /// Method been called in the begining of the execution of thr action
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new { Errors = context.ModelState.Values.SelectMany(x => x.Errors) });
            }
        }
    }
}
