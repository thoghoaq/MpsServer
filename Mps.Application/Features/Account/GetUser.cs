using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Account
{
    public class GetUser
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public string? Email { get; set; }
        }

        public class Result
        {
            public User? User { get; set; }
        }

        public record User
        {
            public int? UserId { get; set; }
            public string? Uid { get; set; }
            public List<string>? Role { get; set; }
            public bool? IsActive { get; set; }
            public Data? Data { get; set; }
        }

        public record Data
        {
            public string? Email { get; set; }
            public string? DisplayName { get; set; }
            public string? PhotoUrl { get; set; }
            public Settings? Settings { get; set; }
            public List<string>? Shortcuts { get; set; }
        }

        public record Settings
        {
            public Layout? Layout { get; set; }
            public Theme? Theme { get; set; }
        }

        public record Layout
        {

        }

        public record Theme
        {

        }

        public class Handler(MpsDbContext dbContext, ILoggedUser loggedUser, IAppLocalizer localizer) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _dbContext = dbContext;
            private readonly ILoggedUser _loggedUser = loggedUser;
            private readonly IAppLocalizer _localizer = localizer;
            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => request.Email != null ? u.Email == request.Email : u.UserId == _loggedUser.UserId, cancellationToken);
                if (user == null)
                {
                    return CommandResult<Result>.Fail(_localizer["User not found"]);
                }
                var result = new Result
                {
                    User = new User
                    {
                        UserId = user.UserId,
                        Uid = user?.IdentityId,
                        Role = user?.Role.Split(",").Where(r => r != "").ToList(),
                        IsActive = user?.IsActive,
                        Data = new Data
                        {
                            Email = user?.Email,
                            DisplayName = user?.FullName,
                            PhotoUrl = user?.AvatarPath,
                            Settings = new Settings
                            {
                                Layout = new Layout(),
                                Theme = new Theme()
                            },
                            Shortcuts = []
                        }
                    }
                };
                return CommandResult<Result>.Success(result);
            }
        }
    }
}
