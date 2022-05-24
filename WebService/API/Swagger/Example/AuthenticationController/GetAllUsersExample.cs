using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using WebService.API.Controllers.Models;

namespace WebService.API.Swagger.Example.AlarmsController
{
    public class GetAllUsersResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            var records = new List<object> {
                new UserModel()
                {
                    Email = "ExampleEmail_1@test.com",
                    Role = 0
                },
                new UserModel()
                {
                    Email = "ExampleEmail_2@test.com",
                    Role = 1
                },

            };

            return records;
        }
    }
}
