using EFCore.BulkExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Payment
{
    public class RequestMonthlyPayout
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public DateTime MonthToDate { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILogger<RequestMonthlyPayout> logger, IAppLocalizer localizer) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var existShop = dbContext.Payouts
                        .Where(p => p.MonthToDate.Month == request.MonthToDate.Month && p.MonthToDate.Year == request.MonthToDate.Year)
                        .Select(p => p.ShopId)
                        .ToList();

                    var PERCENT = 0.9m;

                    // Calculate the total amount threshold
                    decimal thresholdAmount = PERCENT * dbContext.Orders
                        .Where(o => o.OrderDate.Month == request.MonthToDate.Month && o.OrderDate.Year == request.MonthToDate.Year)
                        .Where(o => o.OrderStatusId == (int)Domain.Enums.OrderStatus.Completed)
                        .Sum(o => o.TotalAmount);

                    // Query to update payouts
                    var toUpdatePayouts = dbContext.Payouts
                        .Where(p => p.MonthToDate.Month == request.MonthToDate.Month && p.MonthToDate.Year == request.MonthToDate.Year)
                        .Where(p => p.PayoutStatusId == (int)Domain.Enums.PayoutStatus.Failed ||
                                    p.PayoutStatusId == (int)Domain.Enums.PayoutStatus.Pending ||
                                    thresholdAmount > p.ExpectAmount)
                        .Select(p => new Payout
                        {
                            Id = p.Id,
                            MonthToDate = p.MonthToDate,
                            ShopId = p.ShopId,
                            PayoutStatusId = (int)Domain.Enums.PayoutStatus.Pending,
                            CreatedDate = DateTime.UtcNow,
                            Amount = p.Amount,
                            Currency = p.Currency,
                            UpdatedDate = p.UpdatedDate,
                            BatchId = p.BatchId,
                            ExpectAmount = thresholdAmount - p.Amount
                        })
                        .ToList();
                    await dbContext.BulkUpdateAsync(toUpdatePayouts);

                    var newPayouts = dbContext.Shops
                        .Where(s => s.IsActive && s.PayPalAccount != null)
                        .Where(s => !existShop.Contains(s.Id))
                        .Select(s => new Payout
                        {
                            ShopId = s.Id,
                            MonthToDate = request.MonthToDate,
                            PayoutStatusId = (int)Domain.Enums.PayoutStatus.Pending,
                            CreatedDate = DateTime.UtcNow,
                            ExpectAmount = dbContext.Orders
                                .Where(o => o.ShopId == s.Id)
                                .Where(o => o.OrderDate.Month == request.MonthToDate.Month && o.OrderDate.Year == request.MonthToDate.Year)
                                .Where(o => o.OrderStatusId == (int)Domain.Enums.OrderStatus.Completed)
                                .Sum(o => o.TotalAmount) * PERCENT
                        })
                        .ToList();
                    await dbContext.BulkInsertAsync(newPayouts);
                    return CommandResult<Result>.Success(new Result { Message = localizer["Request monthly payout successfully"] });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(localizer["An error occurred while processing the request"]);
                }
            }
        }
    }
}
