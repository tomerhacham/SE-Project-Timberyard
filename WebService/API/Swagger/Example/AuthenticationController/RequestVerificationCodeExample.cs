using Swashbuckle.AspNetCore.Filters;
using WebService.API.Controllers.Models;

namespace WebService.API.Swagger.Example.AlarmsController
{
    public class RequestVerificationCodeExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new EmailModel()
            {
                Email = "ExampleEmail@test.com"
            };
        }
    }
}
