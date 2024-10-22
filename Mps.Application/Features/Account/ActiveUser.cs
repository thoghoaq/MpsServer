﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Enums;
using Mps.Domain.Entities;
using Mps.Domain.Extensions;

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

        public class CommandHandler(MpsDbContext dbContext, ILogger<ActiveUser> logger, IAppLocalizer localizer, ILoggedUser loggedUser) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _dbContext = dbContext;
            private readonly ILogger<ActiveUser> logger = logger;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILoggedUser _loggedUser = loggedUser;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
                    if (user == null)
                    {
                        return CommandResult<Result>.Fail(_localizer["User not found"]);
                    }
                    if (user.Role.Contains(Role.SuperAdmin.GetDescription())) 
                    {
                        return CommandResult<Result>.Fail(_localizer["You don't have permission to activate or deactivate this user"]);
                    }
                    if (user.Role.Contains(Role.Admin.GetDescription()) && !_loggedUser.Roles.Contains(Role.SuperAdmin.GetDescription()))
                    {
                        return CommandResult<Result>.Fail(_localizer["You don't have permission to activate or deactivate this user"]);
                    }
                    if (user.Role.Contains(Role.Staff.GetDescription()) && !_loggedUser.IsAdminGroup)
                    {
                        return CommandResult<Result>.Fail(_localizer["You don't have permission to activate or deactivate this user"]);
                    }

                    user.IsActive = request.IsActive;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { UserId = user.Id, IsActive = user.IsActive });
                } catch (Exception ex)
                {
                    logger.LogError(ex, "ActiveUserFailure");
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }
        }
    }
}
