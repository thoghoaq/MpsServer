using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Abstractions.Messaging;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Enums;
using Mps.Domain.Extensions;

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

        public class CommandHandler(MpsDbContext context, ILoggedUser loggedUser, IAppLocalizer localizer, ILogger<CreateShop> logger, INotificationService notificationService) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly ILoggedUser _loggedUser = loggedUser;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<CreateShop> _logger = logger;
            private readonly INotificationService _notificationService = notificationService;

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

                    var staffIds = _context.Users.Where(u => u.Role.Contains(Role.Staff.GetDescription()) || u.Role.Contains(Role.Admin.GetDescription()) || u.Role.Contains(Role.SuperAdmin.GetDescription())).Select(u => u.Id).ToList();
                    await _notificationService.SendMessageAllDevicesAsync(staffIds, new MessageRequest
                    {
                        Title = _localizer["New shop request"],
                        Body = _localizer["New shop request from "] + _loggedUser.FullName,
                        ImageUrl = shop.Avatar,
                        Data = new Dictionary<string, string>
                        {
                            { "type", NotificationType.NewShopRequest.ToString() },
                            { "shopId", shop.Id.ToString() }
                        },
                    });
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
