using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Mps.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Mps.Infrastructure.Middleware
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthAttribute : Attribute, IAuthorizationFilter
    {
        public string? Roles { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (Roles.IsNullOrEmpty())
            {
                return;
            }
            var dbContext = context.HttpContext
            .RequestServices
            .GetService(typeof(MpsDbContext)) as MpsDbContext;

            var token = context.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            
            if (token.IsNullOrEmpty())
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var userId = jwtToken?.Payload?.Sub;
            var user = dbContext?.Users.FirstOrDefault(x => x.IdentityId == userId);
            if (user == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            if (!user.Role.Contains(Roles!))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            return;
        }
    }
}
