using Swashbuckle.AspNetCore.Filters;
using WebService.API.Controllers.Models;

namespace WebService.API.Swagger.Example.AuthenticationController
{
    public class RemoveUserExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new EmailModel()
            {
                Email = "ExampleEmail@test.com",
            };
        }
    }
}
