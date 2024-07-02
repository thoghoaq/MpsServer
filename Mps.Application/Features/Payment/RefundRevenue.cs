using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Payment;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using PayoutsSdk.Payouts;

namespace Mps.Application.Features.Payment
{
    public class RefundRevenue
    {
        private static readonly decimal PERCENT = 0.9m;

        public class Command : IRequest<CommandResult<Result>>
        {
            public required List<int> ShopIds { get; set; }
            public required DateTime MonthToDate { get; set; }
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
            public required string Currency { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public required string BatchId { get; set; }
        }

        public class Handler(MpsDbContext dbContext, IPayPalService payPalService, ICurrencyConverter currencyConverter, ILogger<RefundRevenue> logger) : IRequestHandler<Command, CommandResult<Result>>
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
                    logger.LogInformation($"Exchange rate from VND to USD: {vndToUsd}");

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
                        .Select(p => new
                        {
                            p.ShopId,
                            p.ExpectAmount
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
                            EmailMessage = $"SMPS refund your revenue in month {request.MonthToDate.Month}/{request.MonthToDate.Year}",
                            EmailSubject = $"SMPS refund your revenue in month {request.MonthToDate.Month}/{request.MonthToDate.Year}"
                        },
                        Items = groupShopOrders.Select(group =>
                        {
                            var grossInVND = group.ExpectAmount ?? 0;
                            var grossInUSD = Math.Round(grossInVND * vndToUsd * PERCENT, 2);
                            var bankAccount = shopBankAccounts.Find(s => s.Id == group.ShopId)?.PayPalAccount;
                            return new PayoutItem()
                            {
                                RecipientType = "EMAIL",
                                Amount = new Currency()
                                {
                                    CurrencyCode = "USD",
                                    Value = grossInUSD.ToString(),
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
                                Amount = Math.Round((groupShopOrders.Find(x => x.ShopId == shopId)?.ExpectAmount ?? 0) * PERCENT, 2),
                                Currency = "VND",
                                UpdatedDate = DateTime.UtcNow,
                                BatchId = result.Result<CreatePayoutResponse>().BatchHeader.PayoutBatchId
                            }).ToList()
                        })
                        : CommandResult<Result>.Fail("An error occurs when refund revenue");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail("An error occurred");
                }
            }
        }
    }
}
