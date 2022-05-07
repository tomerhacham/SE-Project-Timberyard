﻿using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.API.Controllers.Models;

namespace WebService.API.Swagger.Example.AuthenticationController
{
    public class ForgetPasswordExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new ForgetPasswordModel()
            {
                Email = "ForgetPassword@test.com",
            };
        }
    }
}