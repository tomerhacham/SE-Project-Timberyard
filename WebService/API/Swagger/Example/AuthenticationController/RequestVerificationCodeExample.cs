﻿using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using WebService.API.Controllers.Models;

namespace WebService.API.Swagger.Example.AlarmsController
{
    public class RequestVerificationCodeExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new RequestVerificationCodeModel()
            {
                Email = "ExampleEmail@test.com"
            };
        }
    }
}