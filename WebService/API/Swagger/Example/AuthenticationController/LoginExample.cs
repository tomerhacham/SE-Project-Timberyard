using Swashbuckle.AspNetCore.Filters;
using WebService.API.Controllers.Models;

namespace WebService.API.Swagger.Example.AlarmsController
{
    public class LoginExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new LoginModel()
            {
                Email = "ExampleEmail@test.com",
                Password = "123456"
            };
        }
    }
}
