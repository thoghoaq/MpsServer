using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Seller
{
    public class CreateShop
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required string ShopName { get; set; }
            public required string Address { get; set; }
            public required string PhoneNumber { get; set; }
            public required string PayPalAccount { get; set; }
            public required string City { get; set; }
            public required string District { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string? Description { get; set; }
            public string? Avatar { get; set; }
            public string? Cover { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class CommandHandler(MpsDbContext context, ILoggedUser loggedUser, IAppLocalizer localizer, ILogger<CreateShop> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly ILoggedUser _loggedUser = loggedUser;
            private readonly IAppLocalizer _localizer = localizer;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var shop = new Domain.Entities.Shop
                    {
                        ShopOwnerId = _loggedUser.UserId,
                        ShopName = request.ShopName,
                        Address = request.Address,
                        PhoneNumber = request.PhoneNumber,
                        City = request.City,
                        District = request.District,
                        Latitude = request.Latitude,
                        Longitude = request.Longitude,
                        Description = request.Description,
                        Avatar = request.Avatar,
                        Cover = request.Cover,
                        IsActive = false,
                        IsAccepted = true,
                        PayPalAccount = request.PayPalAccount,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    };
                    _context.Shops.Add(shop);
                    await _context.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result
                    {
                        Message = _localizer["Shop created successfully"]
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "CreateShopFailure");
                    return CommandResult<Result>.Fail(_localizer[ex.Message]);
                }
            }
        }
    }
}
