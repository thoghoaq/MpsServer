using MediatR;
using Microsoft.EntityFrameworkCore;
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
        }

        public class Handler(MpsDbContext dbContext, IPayPalService payPalService, ICurrencyConverter currencyConverter, ILogger<RefundRevenue> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var vndToUsd = await currencyConverter.GetExchangeRateAsync("VND", "USD", cancellationToken);
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

                    // calculate revenue
                    var groupShopOrders = dbContext.Orders
                        .Include(o => o.Shop)
                        .Where(o => o.Shop != null && o.Shop.IsActive && o.Shop.PayPalAccount != null)
                        .Where(o => o.OrderDate.Month == request.MonthToDate.Month && o.OrderDate.Year == request.MonthToDate.Year)
                        .Where(o => o.OrderStatusId == (int)Domain.Enums.OrderStatus.Completed)
                        .GroupBy(o => o.ShopId)
                        .Select(g => new
                        {
                            ShopId = g.Key,
                            TotalAmount = g.Sum(o => o.TotalAmount)
                        })
                        .ToList();
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
                            var grossInVND = group.TotalAmount;
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
                        ? CommandResult<Result>.Success(new Result { Message = "Refund revenue successfully" })
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
