using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Shop
{
    public class ChangeOrderStatus
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required int OrderId { get; set; }
            public required Domain.Enums.OrderStatus Status { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext context, ILogger<ChangeOrderStatus> logger, ILoggedUser loggedUser, IAppLocalizer localizer) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var order = await context.Orders.FindAsync(request.OrderId, cancellationToken);
                    if (order == null)
                    {
                        return CommandResult<Result>.Fail(localizer["Order not found"]);
                    }
                    if (!loggedUser.IsShopOwnerOf(order.ShopId))
                    {
                        return CommandResult<Result>.Fail(localizer["Unauthorized"]);
                    }

                    if (!IsValidStatus(request.Status, (Domain.Enums.OrderStatus)order.OrderStatusId))
                    {
                        return CommandResult<Result>.Fail(localizer["Invalid order status"]);
                    }

                    order.OrderStatusId = (int)request.Status;
                    await context.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Message = localizer["Order status changed"] });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(localizer["Error when change order status"]);
                }
            }

            private bool IsValidStatus(Domain.Enums.OrderStatus requestStatus, Domain.Enums.OrderStatus orderStatus)
            {
                switch (orderStatus)
                {
                    case Domain.Enums.OrderStatus.Processing:
                        return requestStatus == Domain.Enums.OrderStatus.Delivered || requestStatus == Domain.Enums.OrderStatus.Cancelled;
                    case Domain.Enums.OrderStatus.Delivered:
                        return requestStatus == Domain.Enums.OrderStatus.Completed || requestStatus == Domain.Enums.OrderStatus.Returned;
                    case Domain.Enums.OrderStatus.Cancelled:
                        return requestStatus == Domain.Enums.OrderStatus.Refunded;
                    case Domain.Enums.OrderStatus.Returned:
                        return requestStatus == Domain.Enums.OrderStatus.Refunded;
                    default:
                        return false;
                }
            }
        }
    }
}
