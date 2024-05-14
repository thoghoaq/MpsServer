using FirebaseAdmin.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Application.Helpers;
using Mps.Domain.Constants;
using Mps.Domain.Entities;
using Mps.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Mps.Application.Features.Account
{
    public class CreateUser
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            [EmailAddress]
            public required string Email { get; set; }
            [MinLength(6)]
            public required string Password { get; set; }
            public required string FullName { get; set; }
            public required string Role { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(IAuthenticationService authenticationService, MpsDbContext context, ILoggedUser loggedUser, ILogger<CreateUser> logger, IAppLocalizer localizer) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly IAuthenticationService _authenticationService = authenticationService;
            private readonly MpsDbContext _context = context;
            private readonly ILogger<CreateUser> _logger = logger;
            private readonly ILoggedUser _loggedUser = loggedUser;
            private readonly IAppLocalizer _localizer = localizer;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!request.Role.InRoles())
                    {
                        return CommandResult<Result>.Fail(_localizer["Role is not valid"]);
                    }
                    if (request.Role == Role.Admin.GetDescription() && !_loggedUser.IsAdminGroup)
                    {
                        return CommandResult<Result>.Fail(_localizer["You don't have permission to create this role"]);
                    }
                    if (request.Role == Role.Staff.GetDescription() && !_loggedUser.IsAdminGroup)
                    {
                        return CommandResult<Result>.Fail(_localizer["You don't have permission to create this role"]);
                    }
                    var existUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
                    if (existUser != null)
                    {
                        return await AppendNewRole(existUser, request, cancellationToken);
                    }
                    return await CreateNewUser(request, cancellationToken);
                } catch (Exception ex)
                {
                    _logger.LogError(ex, "CreateUserFailure");
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }

            private async Task<CommandResult<Result>> AppendNewRole(User user, Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (user.Role.Contains(request.Role))
                    {
                        return CommandResult<Result>.Fail(_localizer["User already has this role"]);
                    }
                    user.Role = user.Role + request.Role + ",";
                    user.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Message = _localizer["Successfully created new role"] });
                } catch (Exception ex)
                {
                    _logger.LogError(ex, "AppendNewRoleFailure");
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }

            private async Task<CommandResult<Result>> CreateNewUser(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var identityId = await _authenticationService.RegisterAsync(request.Email, request.Password, cancellationToken);
                    if (string.IsNullOrEmpty(identityId))
                    {
                        return CommandResult<Result>.Fail(_localizer["Firebase register return null"]);
                    }
                    var user = new User
                    {
                        Email = request.Email,
                        IdentityId = identityId,
                        Role = request.Role + ",",
                        FullName = request.FullName,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.Add(user);
                    await _context.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Message = _localizer["Successfully created new user"]});
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CreateNewUserFailure");
                    if (ex is not FirebaseAuthException)
                    {
                        await _authenticationService.DeleteAccountAsync(request.Email, cancellationToken);
                    }
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }
        }
    }
}
