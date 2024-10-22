﻿using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Abstractions.Payment;
using Mps.Application.Abstractions.Setting;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Enums;
using PayoutsSdk.Payouts;
using System.Globalization;

namespace Mps.Application.Features.Payment
{
    public class RefundRevenue
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required List<int> ShopIds { get; set; }
            public required DateTime MonthToDate { get; set; }
            public required PayoutDate PayoutDate { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
            public List<PayoutResult>? PayoutResult { get; set; }
        }

        public class PayoutResult
        {
            public int ShopId { get; set; }
            public decimal Amount { get; set; }
            public decimal ExpectAmount { get; set; }
            public required string Currency { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public required string BatchId { get; set; }
        }

        public class Handler(MpsDbContext dbContext, IPayPalService payPalService, ICurrencyConverter currencyConverter, ILogger<RefundRevenue> logger, IAppLocalizer localizer, ISettingService settingService) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var vndToUsd = await currencyConverter.GetExchangeRateAsync("VND", "USD");
                    if (vndToUsd == 0)
                    {
                        return CommandResult<Result>.Fail("Cannot get exchange rate from VND to USD");
                    }

                    var shopBankAccounts = dbContext.Shops
                        .Where(s => request.ShopIds.Contains(s.Id))
                        .Where(s => s.IsActive && s.PayPalAccount != null)
                        .Select(x => new
                        {
                            x.Id,
                            x.PayPalAccount
                        })
                        .ToList();

                    var groupShopOrders = dbContext.Payouts
                        .Where(p => request.ShopIds.Contains(p.ShopId))
                        .Where(p => p.MonthToDate.Month == request.MonthToDate.Month && p.MonthToDate.Year == request.MonthToDate.Year)
                        .Where(p => p.PayoutDate == (int)request.PayoutDate)
                        .Where(p => p.ExpectAmount > 0)
                        .Select(p => new
                        {
                            p.ShopId,
                            p.ExpectAmount,
                            p.Amount
                        }).ToList();
                    if (groupShopOrders.Count == 0)
                    {
                        return CommandResult<Result>.Fail("No revenue to refund");
                    }

                    // refund revenue
                    var payoutRequest = new CreatePayoutRequest()
                    {
                        SenderBatchHeader = new SenderBatchHeader()
                        {
                            EmailMessage = $"SMPS refund your revenue in month {request.MonthToDate.Month}/{request.MonthToDate.Year}. Final settlement date: {request.PayoutDate}",
                            EmailSubject = $"SMPS refund your revenue in month {request.MonthToDate.Month}/{request.MonthToDate.Year}. Final settlement date: {request.PayoutDate}"
                        },
                        Items = groupShopOrders.Select(group =>
                        {
                            var grossInVND = group.ExpectAmount ?? 0;
                            var net = settingService.GetNetAmount(grossInVND);
                            var netInUSD = Math.Round(net * vndToUsd, 2);
                            logger.LogInformation($"Refund revenue for shop {group.ShopId}: {netInUSD.ToString("0.00", CultureInfo.GetCultureInfo("en-US"))} USD");
                            var bankAccount = shopBankAccounts.Find(s => s.Id == group.ShopId)?.PayPalAccount;
                            return new PayoutItem()
                            {
                                RecipientType = "EMAIL",
                                Amount = new Currency()
                                {
                                    CurrencyCode = "USD",

                                    Value = netInUSD.ToString("0.00", CultureInfo.GetCultureInfo("en-US")),
                                },
                                Receiver = bankAccount
                            };
                        }).ToList()
                    };

                    var result = await payPalService.CreatePayoutRequest(payoutRequest);
                    return result.StatusCode == System.Net.HttpStatusCode.Created
                        ? CommandResult<Result>.Success(new Result
                        {
                            Message = "Refund revenue successfully",
                            PayoutResult = request.ShopIds.Select(shopId => new PayoutResult
                            {
                                ShopId = shopId,
                                Amount = (groupShopOrders.Find(x => x.ShopId == shopId)?.ExpectAmount ?? 0) + (groupShopOrders.Find(x => x.ShopId == shopId)?.Amount ?? 0),
                                ExpectAmount = 0,
                                Currency = "VND",
                                UpdatedDate = DateTime.UtcNow,
                                BatchId = result.Result<CreatePayoutResponse>().BatchHeader.PayoutBatchId
                            }).ToList()
                        })
                        : CommandResult<Result>.Fail(localizer["An error occurs when refund revenue"]);
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
