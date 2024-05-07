using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Mps.Application.Features.Account
{
    public class DeleteUser
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            [EmailAddress]
            public required string Email { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(IAuthenticationService authenticationService, MpsDbContext context, ILogger<DeleteUser> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly IAuthenticationService _authenticationService = authenticationService;
            private readonly MpsDbContext _context = context;
            private readonly ILogger<DeleteUser> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
                    if (user == null)
                    {
                        return CommandResult<Result>.Fail("User not found");
                    }
                    await _authenticationService.DeleteAccountAsync(user.IdentityId, cancellationToken);
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Message = "Delete user successfully" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "DeleteUserFailure");
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }
        }
    }
}
