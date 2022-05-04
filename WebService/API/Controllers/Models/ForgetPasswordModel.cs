﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.API.Controllers.ModelValidation;

namespace WebService.API.Controllers.Models
{
    public class ForgetPasswordModel
    {
        [ValidEmail]
        public string Email { get; set; }
    }
}
