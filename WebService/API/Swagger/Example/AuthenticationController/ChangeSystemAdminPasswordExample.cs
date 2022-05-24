using Swashbuckle.AspNetCore.Filters;
using WebService.API.Controllers.Models;

namespace WebService.API.Swagger.Example.AuthenticationController
{
    public class ChangeSystemAdminPasswordExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new UserChangeSystemAdminPasswordModel()
            {
                OldPassword = "01234",
                NewPassword = "56789",
            };
        }
    }
}
