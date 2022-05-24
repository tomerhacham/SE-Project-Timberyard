using Swashbuckle.AspNetCore.Filters;
using WebService.API.Controllers.Models;

namespace WebService.API.Swagger.Example.AuthenticationController
{
    public class ForgetPasswordExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new EmailModel()
            {
                Email = "ForgetPassword@test.com",
            };
        }
    }
}
