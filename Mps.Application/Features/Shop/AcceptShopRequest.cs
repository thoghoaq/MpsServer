using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Shop
{
    public class AcceptShopRequest
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required int ShopId { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class CommandHandler(MpsDbContext context, IAppLocalizer localizer, ILogger<AcceptShopRequest> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var shop = await context.Shops.FindAsync(request.ShopId);
                    if (shop == null)
                    {
                        return CommandResult<Result>.Fail(localizer["ShopNotFound"]);
                    }

                    shop.IsActive = true;
                    await context.SaveChangesAsync(cancellationToken);

                    return CommandResult<Result>.Success(new Result { Message = localizer["ShopAccepted"] });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }
        }
    }
}
