using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Extensions;

namespace Mps.Application.Features.Order
{
    public class DailyCompleteOrders
    {
        public class Command : IRequest<CommandResult<Result>>
        {
        }

        public class Result
        {
            public required bool Success { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILogger<DailyCompleteOrders> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var now = DateTime.UtcNow;
                    var twoDaysAgo = now.AddDays(-2).StartOfDay();
                    var sevenDaysAgo = now.AddDays(-7).StartOfDay();
                    var orders = await dbContext.Orders
                        .Where(o =>
                        (o.OrderStatusId == (int)Domain.Enums.OrderStatus.Received && o.ReceivedAt <= twoDaysAgo)
                        || (o.OrderStatusId == (int)Domain.Enums.OrderStatus.Delivered && o.DeliveryAt <= sevenDaysAgo)
                        )
                        .ToListAsync(cancellationToken);
                    orders.ForEach(o =>
                    {
                        if (o.OrderStatusId == (int)Domain.Enums.OrderStatus.Received)
                        {
                            o.OrderStatusId = (int)Domain.Enums.OrderStatus.Completed;
                        }
                        else if (o.OrderStatusId == (int)Domain.Enums.OrderStatus.Delivered)
                        {
                            o.OrderStatusId = (int)Domain.Enums.OrderStatus.Received;
                        }
                    });
                    await dbContext.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Success = true });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error updating order status");
                    return CommandResult<Result>.Fail("Error updating order status");
                }
            }
        }
    }
}
