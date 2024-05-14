using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
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
            public int? Vnp_TxnRef { get; set; }
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
            public int? PaymentRefId { get; set; }
            public decimal? Amount { get; set; }
            public string? Signature { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILogger<GetPayment> logger, IConfiguration configuration, IVnPayService vnPayService, IAppLocalizer localizer) : IRequestHandler<Query, CommandResult<Result>>
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
                    var resultData = new Result();
                    var secretKey = _configuration["VnPay:HashSecret"] ?? string.Empty;
                    _vnPayService.BindingResponse(request.Vnp_Amount, request.Vnp_BankCode, request.Vnp_BankTranNo, request.Vnp_CardType, request.Vnp_OrderInfo, request.Vnp_TransactionNo, request.Vnp_TransactionStatus, request.Vnp_TxnRef, request.Vnp_SecureHash, request.Vnp_PayDate, request.Vnp_ResponseCode, request.Vnp_TmnCode);
                    //var isValidSignature = _vnPayService.IsValidSignature(secretKey);
                    var payment = await _dbContext.Payments.Include(x => x.PaymentSignature).FirstOrDefaultAsync(x => x.PaymentId == request.Vnp_TxnRef, cancellationToken: cancellationToken);
                    if (payment != null)
                    {
                        resultData.PaymentMessage = _vnPayService.GetResponseMessage(request.Vnp_ResponseCode ?? string.Empty);
                        resultData.PaymentStatus = request.Vnp_ResponseCode;
                        var isSuccess = _vnPayService.IsSuccessResponse(request.Vnp_ResponseCode ?? string.Empty);
                        if (isSuccess)
                        {
                            resultData.PaymentId = payment.PaymentId;
                            resultData.PaymentDate = payment.PaymentDate?.ToString("yyyyMMddHHmmss");
                            resultData.PaymentRefId = payment.PaymentRefId;
                            resultData.Amount = payment.RequiredAmount;
                            resultData.Signature = payment.PaymentSignature?.SignValue;

                            payment.PaymentStatusId = (int)Domain.Enums.PaymentStatus.Success;
                            await _dbContext.SaveChangesAsync(cancellationToken);
                            return CommandResult<Result>.Success(resultData);
                        }
                        payment.PaymentStatusId = (int)Domain.Enums.PaymentStatus.Failed;
                        await _dbContext.SaveChangesAsync(cancellationToken);
                        return CommandResult<Result>.Fail(_localizer[resultData.PaymentMessage]);
                    }
                    return CommandResult<Result>.Fail(_localizer["Payment not found"]);
                } catch (Exception ex)
                {
                    _logger.LogError(ex, "GetPaymentFailure");
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }
        }
    }
}
