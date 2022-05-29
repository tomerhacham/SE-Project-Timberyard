using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// util fucntion to extract token from request header if provided.
        /// In case a token is provided, the embedded information will be extracted too and be assign in the HttpContext
        /// </summary>
        /// <param name="context"></param>
        /// <param name="authController"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, AuthenticationController authController)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var a = context.Request.Headers["Authorization"];

            if (token != null)
            {
                AttachUserToContext(context, authController, token);
            }

            await _next(context);
        }

        /// <summary>
        /// Extracting the relevent information from the users token if its valid.
        /// In cae the token expired and boolean indicate so will be turn on
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userService"></param>
        /// <param name="token"></param>
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
                Logger.Warning($"An error occurred while attempting to attach user to context", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
            }
        }
    }
}