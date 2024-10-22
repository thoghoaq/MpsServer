﻿using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Abstractions.Messaging;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Staff
{
    public class AcceptShop
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public int Id { get; set; }
            public bool IsAccepted { get; set; }
            public string? Comment { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<AcceptShop> logger, INotificationService notificationService) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<AcceptShop> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var shop = await _context.Shops.FindAsync(request.Id);
                    if (shop == null)
                    {
                        return CommandResult<Result>.Fail(_localizer["Shop not found"]);
                    }

                    shop.IsAccepted = request.IsAccepted;
                    if (!request.IsAccepted)
                    {
                        shop.IsActive = false;
                        shop.Comment = request.Comment;
                    }
                    else
                    {
                        shop.IsActive = true;
                        // Apply default shop settings
                        if (!_context.ShopSettings.Any(s => s.ShopId == shop.Id))
                        {
                            var settings = _context.Settings.ToList();
                            _context.ShopSettings.AddRange(settings.Select(s => new ShopSetting { ShopId = shop.Id, Key = s.Key, Value = s.Value, Description = s.Description }));
                        }
                    }

                    await _context.SaveChangesAsync(cancellationToken);

                    // Send notification to shop owner
                    await notificationService.SendMessageAllDevicesAsync(shop.ShopOwnerId, new MessageRequest
                    {
                        Title = request.IsAccepted ? _localizer["Shop Accepted"] : _localizer["Shop Rejected"],
                        Body = request.Comment ?? shop.ShopName,
                        ImageUrl = shop.Avatar
                    });

                    return CommandResult<Result>.Success(new Result { Message = _localizer["Shop accepted/rejected successfully"] });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "AcceptShopFailure");
                    return CommandResult<Result>.Fail(_localizer[ex.Message]);
                }
            }
        }
    }
}
