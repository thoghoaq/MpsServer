using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Account
{
    public class UpdateUser
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public int UserId { get; set; }
            public string? FullName { get; set; }
            public string? PhoneNumber { get; set; }
            public string? AvatarPath { get; set; }
            public StaffData? StaffData { get; set; }
            public CustomerData? CustomerData { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public record StaffData
        {
            public string? IdentityCard { get; set; }
            public string? IdentityCardFrontPath { get; set; }
            public string? IdentityCardBackPath { get; set; }
            public string? Address { get; set; }
            public string? CertificatePath { get; set; }
        }

        public record CustomerData
        {
            public string? Address { get; set; }
        }

        public class Handler(ILoggedUser loggedUser, ILogger<UpdateUser> logger, IAppLocalizer localizer, MpsDbContext dbContext) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly ILogger<UpdateUser> _logger = logger;
            private readonly ILoggedUser _loggedUser = loggedUser;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly MpsDbContext _dbContext = dbContext;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_loggedUser.UserId != request.UserId && !_loggedUser.IsAdminGroup)
                {
                    return CommandResult<Result>.Fail(_localizer["You are not allowed to update this user"]);
                }

                try
                {
                    var user = await _dbContext.Users
                        .Include(x => x.Staff)
                        .Include(x => x.Customer)
                        .Where(x => x.Id == request.UserId)
                        .FirstOrDefaultAsync(cancellationToken);
                    if (user == null)
                    {
                        return CommandResult<Result>.Fail(_localizer["User not found"]);
                    }
                    user.AvatarPath = request.AvatarPath ?? user.AvatarPath;
                    user.FullName = request.FullName ?? user.FullName;
                    user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
                    user.UpdatedAt = DateTime.UtcNow;
                    if (user.Staff != null)
                    {
                        user.Staff.IdentityCard = request.StaffData?.IdentityCard ?? user.Staff.IdentityCard;
                        user.Staff.IdentityCardFrontPath = request.StaffData?.IdentityCardFrontPath ?? user.Staff.IdentityCardFrontPath;
                        user.Staff.IdentityCardBackPath = request.StaffData?.IdentityCardBackPath ?? user.Staff.IdentityCardBackPath;
                        user.Staff.Address = request.StaffData?.Address ?? user.Staff.Address;
                        user.Staff.CertificatePath = request.StaffData?.CertificatePath ?? user.Staff.CertificatePath;
                        user.Staff.UpdatedAt = DateTime.UtcNow;
                    }
                    if (user.Customer != null)
                    {
                        user.Customer.Address = request.CustomerData?.Address ?? user.Customer.Address;
                        user.Customer.UpdatedAt = DateTime.UtcNow;
                    }
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Message = _localizer["User updated successfully"] });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "UpdateUserFailure");
                    return CommandResult<Result>.Fail(_localizer["An error occurred while updating user"]);
                }
            }
        }
    }
}
