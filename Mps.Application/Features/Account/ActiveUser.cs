using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Account
{
    public class ActiveUser
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public int UserId { get; set; }
            public bool IsActive { get; set; }
        }

        public class Result
        {
            public int UserId { get; set; }
            public bool IsActive { get; set; }
        }

        public class CommandHandler(MpsDbContext dbContext, ILogger<ActiveUser> logger, IAppLocalizer localizer) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _dbContext = dbContext;
            private readonly ILogger<ActiveUser> logger = logger;
            private readonly IAppLocalizer _localizer = localizer;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
                    if (user == null)
                    {
                        return CommandResult<Result>.Fail(_localizer["User not found"]);
                    }
                    user.IsActive = request.IsActive;
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { UserId = user.UserId, IsActive = user.IsActive });
                } catch (Exception ex)
                {
                    logger.LogError(ex, "ActiveUserFailure");
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }
        }
    }
}
