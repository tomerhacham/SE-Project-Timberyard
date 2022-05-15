﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using WebService.Domain.DataAccess.DTO;
using WebService.Utils;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    Role AuthorizedRole { get; }

    public AuthorizeAttribute(Role authorizedRole = Role.RegularUser)
    {
        AuthorizedRole = authorizedRole;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var email = (string)context.HttpContext.Items["Email"];
        var role = (int)context.HttpContext.Items["Role"];
        if (string.IsNullOrEmpty(email) || role - (int)AuthorizedRole < 0)
        {
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}