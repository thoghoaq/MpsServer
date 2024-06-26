using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Payment
{
    public class AcceptPayout
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required int PayoutId { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext dbContext, IMediator mediator, ILogger<AcceptPayout> logger, IAppLocalizer localizer) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var payout = await dbContext.Payouts.FirstOrDefaultAsync(p => p.Id == request.PayoutId, cancellationToken);
                    if (payout == null)
                    {
                        return CommandResult<Result>.Fail(localizer["Payout not found"]);
                    }
                    if (payout.PayoutStatusId != (int)Domain.Enums.PayoutStatus.Pending)
                    {
                        return CommandResult<Result>.Fail(localizer["Payout is not pending"]);
                    }
                    var refundRevenueCommand = new RefundRevenue.Command
                    {
                        ShopIds = new List<int> { payout.ShopId },
                        MonthToDate = payout.MonthToDate
                    };
                    var refundRevenueResult = await mediator.Send(refundRevenueCommand, cancellationToken);
                    payout.PayoutStatusId = refundRevenueResult.IsSuccess ? (int)Domain.Enums.PayoutStatus.Success : (int)Domain.Enums.PayoutStatus.Failed;
                    await dbContext.SaveChangesAsync(cancellationToken);
                    return refundRevenueResult.IsSuccess
                        ? CommandResult<Result>.Success(new Result { Message = localizer["Payout has been accepted"] })
                        : CommandResult<Result>.Fail(refundRevenueResult.FailureReason ?? localizer["Payout has been rejected"]);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(localizer["An error occurred"]);
                }
            }
        }
    }
}
