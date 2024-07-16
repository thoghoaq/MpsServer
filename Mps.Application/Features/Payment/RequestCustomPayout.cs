using EFCore.BulkExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Abstractions.Setting;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Enums;
using Mps.Domain.Extensions;

namespace Mps.Application.Features.Payment
{
    public class RequestCustomPayout
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required DateTime MonthToDate { get; set; }
            public required PayoutDate PayoutDate { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILogger<RequestMonthlyPayout> logger, IAppLocalizer localizer, ISettingService settingService) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var existShop = dbContext.Payouts
                        .Where(p => p.MonthToDate.Month == request.MonthToDate.Month && p.MonthToDate.Year == request.MonthToDate.Year && p.PayoutDate == (int)request.PayoutDate)
                        .Select(p => p.ShopId)
                        .ToList();

                    var settings = dbContext.Settings.ToList();
                    var allOrders = dbContext.Orders
                        .Where(o => o.OrderStatusId == (int)Domain.Enums.OrderStatus.Completed)
                        .AsEnumerable()
                        .Where(p => p.OrderDate.InPayoutDate(request.MonthToDate, request.PayoutDate))
                        .ToList();

                    var toUpdatePayouts = dbContext.Payouts
                        .Where(p => p.MonthToDate.Month == request.MonthToDate.Month && p.MonthToDate.Year == request.MonthToDate.Year)
                        .Where(p => p.PayoutDate == (int)request.PayoutDate)
                        .AsEnumerable()
                        .Where(p => p.PayoutStatusId == (int)Domain.Enums.PayoutStatus.Failed || p.PayoutStatusId == (int)Domain.Enums.PayoutStatus.Pending ||
                            allOrders
                            .Where(o => o.ShopId == p.ShopId)
                            .Sum(o => settingService.GetNetBySetting(o.TotalAmount, settings)) > p.Amount)
                        .Select(p => new Payout
                        {
                            Id = p.Id,
                            MonthToDate = p.MonthToDate,
                            PayoutDate = p.PayoutDate,
                            ShopId = p.ShopId,
                            PayoutStatusId = (int)Domain.Enums.PayoutStatus.Pending,
                            CreatedDate = DateTime.UtcNow,
                            Amount = p.Amount,
                            Currency = p.Currency,
                            UpdatedDate = p.UpdatedDate,
                            BatchId = p.BatchId,
                            ExpectAmount = allOrders
                            .Where(o => o.ShopId == p.ShopId)
                            .Sum(o => settingService.GetNetBySetting(o.TotalAmount, settings)) - p.Amount
                        })
                        .ToList();
                    await dbContext.BulkUpdateAsync(toUpdatePayouts);

                    var newPayouts = dbContext.Shops
                        .Where(s => s.IsActive && s.PayPalAccount != null)
                        .Where(s => !existShop.Contains(s.Id))
                        .AsEnumerable()
                        .Select(s => new Payout
                        {
                            ShopId = s.Id,
                            MonthToDate = request.MonthToDate,
                            PayoutDate = (int)request.PayoutDate,
                            PayoutStatusId = (int)Domain.Enums.PayoutStatus.Pending,
                            CreatedDate = DateTime.UtcNow,
                            Amount = 0,
                            ExpectAmount = allOrders
                            .Where(o => o.ShopId == s.Id)
                            .Sum(o => settingService.GetNetBySetting(o.TotalAmount, settings))
                        })
                        .ToList();
                    await dbContext.BulkInsertAsync(newPayouts);
                    return CommandResult<Result>.Success(new Result { Message = localizer["Request custom payout successfully"] });
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
