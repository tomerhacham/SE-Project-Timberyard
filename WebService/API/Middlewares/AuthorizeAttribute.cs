using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using WebService.Domain.DataAccess.DTO;

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
        var email = context.HttpContext.Items["Email"];
        var role = context.HttpContext.Items["Role"];
        if (email == string.Empty)
        {
            // not logged in
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
        else if (!AuthorizedRole.Equals(role))
        {
            // Role is not authorized
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}