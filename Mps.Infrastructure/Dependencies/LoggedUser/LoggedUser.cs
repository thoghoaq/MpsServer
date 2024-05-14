using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Mps.Application.Abstractions.Authentication;
using Mps.Domain.Constants;
using Mps.Domain.Entities;
using Mps.Domain.Extensions;
using System.IdentityModel.Tokens.Jwt;

namespace Mps.Infrastructure.Dependencies.LoggedUser
{
    public class LoggedUser(MpsDbContext dbContext, IHttpContextAccessor httpContext) : ILoggedUser
    {
        private readonly MpsDbContext _dbContext = dbContext;
        private readonly IHttpContextAccessor _httpContext = httpContext;
        private User? _user;

        private User? GetUser()
        {
            if (_user == null)
            {
                var token = _httpContext.HttpContext?.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

                if (token.IsNullOrEmpty())
                {
                    throw new UnauthorizedAccessException();
                }
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
                var userId = jwtToken?.Payload?.Sub;
                _user = _dbContext?.Users.FirstOrDefault(x => x.IdentityId == userId);
            }
            return _user;
        }

        public int UserId => GetUser()!.UserId;

        public string FullName => GetUser()!.FullName;

        public string Email => GetUser()!.Email;

        public IEnumerable<string> Roles => GetUser()!.Role.Split(",").Where(r => !r.IsNullOrEmpty());

        public string IdentityId => GetUser()!.IdentityId;

        public string IpAddress => _httpContext.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        public bool IsAuthenticated => GetUser() != null;

        public bool IsManagerGroup => GetUser()!.Role.Contains(Role.Admin.GetDescription()) || GetUser()!.Role.Contains(Role.Staff.GetDescription());

        public bool IsAdminGroup => GetUser()!.Role.Contains(Role.Admin.GetDescription());
    }
}
