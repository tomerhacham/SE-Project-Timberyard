﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebService.API.Controllers.ModelValidation;
using WebService.Domain.Business.Alarms;

namespace WebService.API.Controllers.Models
{
    public class AdminLoginModel
    {
        [ValidEmail]
        public string Email { get; set; }
        [StringIsNotNullOrEmpty]
        public string Password { get; set; }
    }

    public class RequestVerificationCodeModel
    {
        [ValidEmail]
        public string Email { get; set; }
    }

    public class RegularUserLoginModel
    {
        [ValidEmail]
        public string Email { get; set; }
        [StringIsNotNullOrEmpty]
        public string VerificationCode { get; set; }
    }
}