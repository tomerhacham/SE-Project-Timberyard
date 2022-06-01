using Swashbuckle.AspNetCore.Filters;
using WebService.API.Controllers.Models;
using WebService.Domain.DataAccess.DTO;
using WebService.Utils;

namespace WebService.API.Swagger.Example.AlarmsController
{
    public class LoginRequestExample : IExamplesProvider<object>
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

    public class LoginResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new JWTtoken()
            {
                Token = "svkuhyiuq2973nsk20snAd2b0mAskdr2Dkddloisn37dskv0vnzRo",
                Role = Role.RegularUser.ToString()
            };
        }
    }
}
