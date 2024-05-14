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
            public CustomerInfo? CustomerInfo { get; set; }
            public ShopOwnerInfo? ShopOwnerInfo { get; set; }
        }

        public class CustomerInfo
        {
            
        }

        public class ShopOwnerInfo
        {

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
                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
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
                    Role = user.Role
                };
                if (user.Role.Contains(Role.Customer.GetDescription()))
                {
                    var customer = await _dbContext.Customers.FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
                    result.CustomerInfo = new CustomerInfo
                    {
                        
                    };
                }
                if (user.Role.Contains(Role.ShopOwner.GetDescription()))
                {
                    var shopOwner = await _dbContext.ShopOwners.FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
                    result.ShopOwnerInfo = new ShopOwnerInfo
                    {
                        
                    };
                }

                return CommandResult<Result>.Success(result);
            }
        }
    }
}
