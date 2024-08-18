using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Ecommerce
{
    public class UpdateOrderStatusReceived
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public int OrderId { get; set; }
        }

        public class Result
        {
            public required bool Success { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILogger<UpdateOrderStatusReceived> logger, IAppLocalizer localizer) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var order = await dbContext.Orders
                        .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

                    if (order == null)
                    {
                        return CommandResult<Result>.Fail(localizer["Order not found"]);
                    }

                    if (order.OrderStatusId != (int)Domain.Enums.OrderStatus.Delivered)
                    {
                        return CommandResult<Result>.Fail(localizer["Order status is not Delivered"]);
                    }

                    order.OrderStatusId = (int)Domain.Enums.OrderStatus.Completed;
                    order.ReceivedAt = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync(cancellationToken);

                    return CommandResult<Result>.Success(new Result { Success = true });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error updating order status");
                    return CommandResult<Result>.Fail(localizer["Error updating order status"]);
                }
            }
        }
    }
}
