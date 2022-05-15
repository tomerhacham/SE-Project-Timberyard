using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebService.Domain.Business.Authentication;
using WebService.Domain.DataAccess.DTO;
using WebService.Utils;
using WebService.Utils.Models;

namespace WebService.API.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string Secret;
        private readonly ILogger Logger;

        public JwtMiddleware(RequestDelegate next, IOptions<AuthenticationSettings> settings, ILogger logger)
        {
            _next = next;
            Secret = settings.Value.Secret;
            Logger = logger;
        }

        public async Task Invoke(HttpContext context, AuthenticationController authController)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                AttachUserToContext(context, authController, token);
            }

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, AuthenticationController userService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Claims.First(x => x.Type == "Email").Value;
                var role = Enum.Parse(typeof(Role), jwtToken.Claims.First(x => x.Type == "Role").Value);

                // attach user to context on successful jwt validation
                context.Items["Email"] = email;
                context.Items["Role"] = role;
                context.Items["ValidLifetime"] = true;
            }
            catch (Exception e)
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
                context.Items["ValidLifetime"] = false;
            }
        }
    }
}