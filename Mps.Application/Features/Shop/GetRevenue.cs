using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Shop
{
    public class GetRevenue
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public required int ShopId { get; set; }
            public DateTime? From { get; set; }
            public DateTime? To { get; set; }
        }

        public class Result
        {
            public List<Revenue> Revenues { get; set; } = [];
            public required decimal Revenue { get; set; }
            public required string Currency { get; set; }
        }

        public class Revenue
        {
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<GetRevenue> logger, ILoggedUser loggedUser) : IRequestHandler<Query, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!loggedUser.IsShopOwnerOf(request.ShopId) && !loggedUser.IsManagerGroup)
                    {
                        return CommandResult<Result>.Fail(localizer["Unauthorized"]);
                    }
                    var shop = await context.Shops.FindAsync(request.ShopId);
                    if (shop == null)
                    {
                        return CommandResult<Result>.Fail(localizer["Shop not found"]);
                    }

                    // get monthly revenue
                    var revenues = context.Orders
                        .Where(o => o.ShopId == request.ShopId)
                        .Where(o => request.From == null || o.OrderDate >= request.From)
                        .Where(o => request.To == null || o.OrderDate <= request.To)
                        .Where(o => o.OrderStatusId == (int)Domain.Enums.OrderStatus.Completed)
                        .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                        .Select(g => new Revenue
                        {
                            Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                            Amount = g.Sum(o => o.TotalAmount),
                        })
                        .ToList();

                    return CommandResult<Result>.Success(new Result
                    {
                        Revenues = revenues,
                        Revenue = revenues.Sum(r => r.Amount),
                        Currency = "VND"
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(localizer["Error when get revenue"]);
                }
            }
        }
    }
}
