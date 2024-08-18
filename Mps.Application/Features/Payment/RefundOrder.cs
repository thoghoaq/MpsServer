using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Abstractions.Payment;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Newtonsoft.Json;

namespace Mps.Application.Features.Payment
{
    public class RefundOrder
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required int OrderId { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext context, ILogger<RefundOrder> logger, IAppLocalizer localizer, IVnPayService vnPayService, IConfiguration configuration, ILoggedUser loggedUser) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
                    if (order == null)
                    {
                        return CommandResult<Result>.Fail(localizer["Order not found"]);
                    }
                    if (order.OrderStatusId != (int)Domain.Enums.OrderStatus.Processing && order.OrderStatusId != (int)Domain.Enums.OrderStatus.Returned)
                    {
                        return CommandResult<Result>.Fail(localizer["Order status is invalid"]);
                    }
                    var payment = await context.Payments.Include(x => x.PaymentRefs).Where(p => p.PaymentRefs.Any(r => r.RefId == order.Id)).FirstOrDefaultAsync(cancellationToken);
                    if (payment == null)
                    {
                        return CommandResult<Result>.Fail(localizer["Payment not found"]);
                    }
                    if (payment.PaymentStatusId != (int)Domain.Enums.PaymentStatus.Success)
                    {
                        return CommandResult<Result>.Fail(localizer["Payment status is invalid"]);
                    }
                    if (payment.PaymentRefs.Count == 0)
                    {
                        return CommandResult<Result>.Fail(localizer["Payment reference not found"]);
                    }
                    if (payment.TransactionNo == null || payment.OrderInfo == null)
                    {
                        return CommandResult<Result>.Fail(localizer["Payment information not found"]);
                    }

                    var vnpSecretKey = configuration["VnPay:HashSecret"]!;
                    var vnpTmnCode = configuration["VnPay:TmnCode"]!;
                    var vnpUrl = configuration["VnPay:RefundApi"]!;
                    var refundResult = await vnPayService.RefundPaymentAsync(
                        vnpSecretKey,
                        vnpTmnCode,
                        vnpUrl,
                        payment.Id.ToString(),
                        (long)order.TotalAmount,
                        localizer[$"Refund for order "] + order.Id,
                        payment.TransactionNo,
                        payment.PaymentDate!.Value.ToString("yyyyMMddHHmmss"),
                        loggedUser.FullName,
                        loggedUser.IpAddress
                        );
                    if (!refundResult.Contains("vnp_ResponseCode"))
                    {
                        return CommandResult<Result>.Fail(localizer["Error has occur"]);
                    }
                    var refundJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(refundResult)!;
                    if (refundJson["vnp_ResponseCode"] != "00")
                    {
                        return CommandResult<Result>.Fail(localizer["Refund failed"]);
                    }
                    return CommandResult<Result>.Success(new Result
                    {
                        Message = localizer["Refund success"]
                    });
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
