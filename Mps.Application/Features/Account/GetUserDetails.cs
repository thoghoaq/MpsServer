using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Enums;
using Mps.Domain.Entities;
using Mps.Domain.Extensions;

namespace Mps.Application.Features.Account
{
    public class GetUserDetails
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int UserId { get; set; }
        }

        public class Result
        {
            public string? FullName { get; set; }
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
            public string? AvatarPath { get; set; }
            public string? Role { get; set; }
            public CustomerData? CustomerData { get; set; }
            public ShopOwnerData? ShopOwnerData { get; set; }
            public StaffData? StaffData { get; set; }
        }

        public class CustomerData
        {
            
        }

        public class ShopOwnerData
        {

        }

        public class StaffData
        {
            public string? IdentityCard { get; set; }
            public string? IdentityCardFrontPath { get; set; }
            public string? IdentityCardBackPath { get; set; }
            public string? Address { get; set; }
            public string? CertificatePath { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILoggedUser loggedUser, IAppLocalizer localizer) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _dbContext = dbContext;
            private readonly ILoggedUser _loggedUser = loggedUser;
            private readonly IAppLocalizer _localizer = localizer;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_loggedUser.UserId != request.UserId && !_loggedUser.IsManagerGroup)
                {
                    return CommandResult<Result>.Fail(_localizer["You are not authorized to view this user"]);
                }
                var user = await _dbContext.Users
                    .Include(x => x.Customer)
                    .Include(x => x.ShopOwner)
                    .Include(x => x.Staff)
                    .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
                if (user == null)
                {
                    return CommandResult<Result>.Fail("User not found");
                }
                var result = new Result
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AvatarPath = user.AvatarPath,
                    Role = user.Role,
                    CustomerData = user.Customer != null ? new CustomerData
                    {
                        
                    } : null,
                    ShopOwnerData = user.ShopOwner != null ? new ShopOwnerData
                    {
                        
                    } : null,
                    StaffData = user.Staff != null ? new StaffData
                    {
                        IdentityCard = user.Staff.IdentityCard,
                        IdentityCardFrontPath = user.Staff.IdentityCardFrontPath,
                        IdentityCardBackPath = user.Staff.IdentityCardBackPath,
                        Address = user.Staff.Address,
                        CertificatePath = user.Staff.CertificatePath,
                        CreatedAt = user.Staff.CreatedAt,
                        UpdatedAt = user.Staff.UpdatedAt
                    } : null
                };

                return CommandResult<Result>.Success(result);
            }
        }
    }
}
