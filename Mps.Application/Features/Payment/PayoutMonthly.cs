using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Payment;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using PayoutsSdk.Payouts;
using System.Globalization;

namespace Mps.Application.Features.Payment
{
    public class PayoutMonthly
    {
        private static readonly decimal PERCENT = 0.9m;

        public class Command : IRequest<CommandResult<Result>>
        {
            public required DateTime MonthToDate { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext dbContext, IPayPalService payPalService, ICurrencyConverter currencyConverter, ILogger<PayoutMonthly> logger, IServiceProvider serviceProvider) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                // get payout request
                var payouts = dbContext.Payouts
                    .Where(p => p.PayoutStatusId == (int)Domain.Enums.PayoutStatus.Pending)
                    .Where(p => p.MonthToDate.Month == request.MonthToDate.Month && p.MonthToDate.Year == request.MonthToDate.Year)
                    .ToList();

                return await Payout(request, payouts);
            }

            private async Task<CommandResult<Result>> Payout(Command request, List<Payout> payouts)
            {
                try
                {
                    // calculate revenue
                    var groupShopOrders = payouts
                        .GroupBy(p => p.ShopId)
                        .Select(group => new
                        {
                            ShopId = group.Key,
                            TotalAmount = group.Sum(x => x.ExpectAmount),
                            Amount = group.Sum(x => x.Amount)
                        })
                        .ToList();
                    if (groupShopOrders.Count == 0)
                    {
                        return CommandResult<Result>.Success(new Result
                        {
                            Message = "No revenue to refund"
                        });
                    }

                    var shopBankAccounts = dbContext.Shops
                        .Where(s => groupShopOrders.Select(x => x.ShopId).Contains(s.Id))
                        .Where(s => s.IsActive && s.PayPalAccount != null)
                        .Select(x => new
                        {
                            x.Id,
                            x.PayPalAccount
                        })
                        .ToList();

                    var vndToUsd = await currencyConverter.GetExchangeRateAsync("VND", "USD");
                    if (vndToUsd == 0)
                    {
                        return CommandResult<Result>.Fail("Cannot get exchange rate from VND to USD");
                    }

                    // refund revenue
                    var payoutRequest = new CreatePayoutRequest()
                    {
                        SenderBatchHeader = new SenderBatchHeader()
                        {
                            EmailMessage = $"SMPS refund your revenue in month {request.MonthToDate.Month}/{request.MonthToDate.Year}",
                            EmailSubject = $"SMPS refund your revenue in month {request.MonthToDate.Month}/{request.MonthToDate.Year}"
                        },
                        Items = groupShopOrders.Select(group =>
                        {
                            var grossInVND = group.TotalAmount ?? 0;
                            var grossInUSD = Math.Round(grossInVND * vndToUsd * PERCENT, 2);
                            var bankAccount = shopBankAccounts.Find(s => s.Id == group.ShopId)?.PayPalAccount;
                            return new PayoutItem()
                            {
                                RecipientType = "EMAIL",
                                Amount = new Currency()
                                {
                                    CurrencyCode = "USD",
                                    Value = grossInUSD.ToString("0.00", CultureInfo.GetCultureInfo("en-US")),
                                },
                                Receiver = bankAccount
                            };
                        }).ToList()
                    };

                    var result = await payPalService.CreatePayoutRequest(payoutRequest);
                    foreach (var payout in payouts)
                    {
                        payout.PayoutStatusId = result.StatusCode == System.Net.HttpStatusCode.Created ? (int)Domain.Enums.PayoutStatus.Success : (int)Domain.Enums.PayoutStatus.Failed;
                        payout.Amount = groupShopOrders.Find(x => x.ShopId == payout.ShopId)?.TotalAmount + groupShopOrders.Find(x => x.ShopId == payout.ShopId)?.Amount;
                        payout.Currency = "VND";
                        payout.UpdatedDate = DateTime.UtcNow;
                        payout.BatchId = result.Result<CreatePayoutResponse>().BatchHeader.PayoutBatchId;
                    }
                    await dbContext.SaveChangesAsync();
                    return result.StatusCode == System.Net.HttpStatusCode.Created
                        ? CommandResult<Result>.Success(new Result { Message = "Refund revenue successfully" })
                        : CommandResult<Result>.Fail("An error occurs when refund revenue");
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
