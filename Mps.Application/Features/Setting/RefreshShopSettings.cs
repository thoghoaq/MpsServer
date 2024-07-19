using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Setting
{
    public class RefreshShopSettings
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required int ShopId { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<RefreshShopSettings> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<RefreshShopSettings> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var shop = await _context.Shops.FindAsync(request.ShopId);
                    if (shop == null)
                    {
                        return CommandResult<Result>.Fail(_localizer["Shop not found"]);
                    }

                    var settings = _context.Settings.ToList();
                    var shopSettings = _context.ShopSettings.Where(s => s.ShopId == shop.Id).ToList();
                    foreach (var setting in settings)
                    {
                        var shopSetting = shopSettings.Find(s => s.Key == setting.Key);
                        if (shopSetting == null)
                        {
                            _context.ShopSettings.Add(new ShopSetting { ShopId = shop.Id, Key = setting.Key, Value = setting.Value, Description = setting.Description });
                        }
                        else
                        {
                            shopSetting.Value = setting.Value;
                            shopSetting.Description = setting.Description;
                        }
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Message = _localizer["Shop settings refreshed successfully"] });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RefreshShopSettingsFailure");
                    return CommandResult<Result>.Fail(_localizer[ex.Message]);
                }
            }
        }
    }
}
