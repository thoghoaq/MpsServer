using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Abstractions.Messaging;
using Mps.Application.Abstractions.Payment;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Enums;

namespace Mps.Application.Features.Payment
{
    public class GetPayment
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public string? Vnp_TmnCode { get; set; }
            public string? Vnp_BankCode { get; set; }
            public string? Vnp_BankTranNo { get; set; }
            public string? Vnp_CardType { get; set; }
            public string? Vnp_OrderInfo { get; set; }
            public string? Vnp_TransactionNo { get; set; }
            public string? Vnp_TransactionStatus { get; set; }
            public string? Vnp_TxnRef { get; set; }
            public string? Vnp_SecureHash { get; set; }
            public int? Vnp_Amount { get; set; }
            public string? Vnp_PayDate { get; set; }
            public string? Vnp_ResponseCode { get; set; }
        }

        public class Result
        {
            public int? PaymentId { get; set; }
            public string? PaymentStatus { get; set; }
            public string? PaymentMessage { get; set; }
            public string? PaymentDate { get; set; }
            public List<int?> PaymentRefId { get; set; } = [];
            public decimal? Amount { get; set; }
            public string? Signature { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILogger<GetPayment> logger, IConfiguration configuration, IVnPayService vnPayService, IAppLocalizer localizer, INotificationService notificationService) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _dbContext = dbContext;
            private readonly ILogger<GetPayment> _logger = logger;
            private readonly IConfiguration _configuration = configuration;
            private readonly IVnPayService _vnPayService = vnPayService;
            private readonly IAppLocalizer _localizer = localizer;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var paymentResult = new Result();
                    var secretKey = _configuration["VnPay:HashSecret"] ?? string.Empty;
                    _vnPayService.BindingResponse(request.Vnp_Amount, request.Vnp_BankCode, request.Vnp_BankTranNo, request.Vnp_CardType, request.Vnp_OrderInfo, request.Vnp_TransactionNo, request.Vnp_TransactionStatus, request.Vnp_TxnRef, request.Vnp_SecureHash, request.Vnp_PayDate, request.Vnp_ResponseCode, request.Vnp_TmnCode);
                    //var isValidSignature = _vnPayService.IsValidSignature(secretKey);
                    if (string.IsNullOrEmpty(request.Vnp_TxnRef))
                    {
                        return CommandResult<Result>.Fail(_localizer["Payment not found"]);
                    }
                    var requestId = int.Parse(request.Vnp_TxnRef);
                    var payment = await _dbContext.Payments.Include(x => x.PaymentSignature).Include(x => x.PaymentRefs).FirstOrDefaultAsync(x => requestId == x.Id, cancellationToken);
                    if (payment == null)
                    {
                        return CommandResult<Result>.Fail(_localizer["Payment not found"]);
                    }

                    var paymentMessage = _vnPayService.GetResponseMessage(request.Vnp_ResponseCode ?? string.Empty);
                    var isSuccess = _vnPayService.IsSuccessResponse(request.Vnp_ResponseCode ?? string.Empty);
                    paymentResult.PaymentMessage = paymentMessage;
                    paymentResult.PaymentStatus = request.Vnp_ResponseCode;

                    var orderIds = payment.PaymentRefs.Select(x => x.RefId).ToList();
                    if (isSuccess)
                    {
                        paymentResult.PaymentId = payment.Id;
                        paymentResult.PaymentDate = payment.PaymentDate?.ToString("yyyyMMddHHmmss");
                        paymentResult.PaymentRefId = payment.PaymentRefs.Select(x => x.RefId).ToList();
                        paymentResult.Amount = payment.RequiredAmount;
                        paymentResult.Signature = payment.PaymentSignature?.SignValue;

                        payment.PaymentStatusId = (int)Domain.Enums.PaymentStatus.Success;

                        var orderDetails = _dbContext.OrderDetails
                            .Include(x => x.Product)
                            .Where(x => orderIds.Contains(x.OrderId))
                            .ToList();

                        foreach (var orderDetail in orderDetails)
                        {
                            orderDetail.Product!.Stock -= orderDetail.Quantity;
                            orderDetail.Product.SoldCount += orderDetail.Quantity;
                        }
                    }
                    else
                    {
                        payment.PaymentStatusId = (int)Domain.Enums.PaymentStatus.Failed;
                    }

                    var orders = await ChangeOrderStatus(orderIds, isSuccess ? (int)Domain.Enums.OrderStatus.Processing : (int)Domain.Enums.OrderStatus.Cancelled, cancellationToken);
                    if (isSuccess)
                    {
                        foreach (var item in orders)
                        {
                            if (item.Shop?.ShopOwnerId != null)
                            {
                                await notificationService.SendMessageAllDevicesAsync(item.Shop.ShopOwnerId, new MessageRequest
                                {
                                    Title = _localizer["You has new order"],
                                    Body = _localizer["Order date"] + ":" + item.OrderDate.ToString() + "UTC",
                                    Data = new Dictionary<string, string>
                                    {
                                        { "type", NotificationType.NewOrder.ToString() },
                                        { "orderId", item.Id.ToString() }
                                    }
                                });
                            }
                        }
                        return CommandResult<Result>.Success(paymentResult);
                    }
                    return CommandResult<Result>.Fail(_localizer[paymentMessage]);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "GetPaymentFailure");
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }

            private async Task<List<Order>> ChangeOrderStatus(List<int?> orderId, int status, CancellationToken cancellationToken)
            {
                var orders = _dbContext.Orders
                    .Include(x => x.Shop)
                    .Where(x => orderId.Contains(x.Id));
                if (orders.Any())
                {
                    foreach (var item in orders)
                    {
                        item.OrderStatusId = status;
                    }
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                return orders.ToList();
            }
        }
    }
}
