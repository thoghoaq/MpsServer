﻿using EFCore.BulkExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Abstractions.Setting;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Extensions;

namespace Mps.Application.Features.Payment
{
    public class RequestWeeklyPayout
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public DateTime WeekToDate { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILogger<RequestWeeklyPayout> logger, IAppLocalizer localizer, ISettingService settingService) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var existShop = dbContext.Payouts
                        .AsEnumerable()
                        .Where(p => p.MonthToDate.CompareWeek(request.WeekToDate))
                        .Select(p => p.ShopId)
                        .ToList();

                    var settings = dbContext.Settings.ToList();
                    var allOrders = dbContext.Orders.ToList();

                    var toUpdatePayouts = dbContext.Payouts
                        .AsEnumerable()
                        .Where(p => p.MonthToDate.CompareWeek(request.WeekToDate))
                        .Where(p => p.PayoutStatusId == (int)Domain.Enums.PayoutStatus.Failed || p.PayoutStatusId == (int)Domain.Enums.PayoutStatus.Pending ||
                            allOrders
                            .Where(o => o.ShopId == p.ShopId)
                            .Where(o => o.OrderStatusId == (int)Domain.Enums.OrderStatus.Completed)
                            .Where(o => o.OrderDate.CompareWeek(request.WeekToDate))
                            .Sum(o => settingService.GetNetBySetting(o.TotalAmount, settings)) > p.Amount)
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
                            ExpectAmount = allOrders
                            .Where(o => o.ShopId == p.ShopId)
                            .Where(o => o.OrderStatusId == (int)Domain.Enums.OrderStatus.Completed)
                            .Where(o => o.OrderDate.CompareWeek(request.WeekToDate))
                            .Sum(o => settingService.GetNetBySetting(o.TotalAmount, settings)) - p.Amount,
                        })
                        .ToList();
                    await dbContext.BulkUpdateAsync(toUpdatePayouts);

                    var newPayouts = dbContext.Shops
                       .Where(s => s.IsActive && s.PayPalAccount != null)
                       .Where(s => !existShop.Contains(s.Id))
                       .ToList()
                       .AsEnumerable()
                       .Select(s => new Payout
                       {
                           ShopId = s.Id,
                           MonthToDate = request.WeekToDate,
                           PayoutStatusId = (int)Domain.Enums.PayoutStatus.Pending,
                           CreatedDate = DateTime.UtcNow,
                           Amount = 0,
                           ExpectAmount = allOrders
                            .Where(o => o.ShopId == s.Id)
                            .Where(o => o.OrderStatusId == (int)Domain.Enums.OrderStatus.Completed)
                            .Where(o => o.OrderDate.CompareWeek(request.WeekToDate))
                            .Sum(o => settingService.GetNetBySetting(o.TotalAmount, settings))
                       })
                       .ToList();
                    await dbContext.BulkInsertAsync(newPayouts);
                    return CommandResult<Result>.Success(new Result { Message = localizer["Request weekly payout successfully"] });
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
