using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Payment;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Payment
{
    public class CreatePayment
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public string? PaymentContent { get; set; }
            public string? PaymentCurrency { get; set; }
            public int? PaymentRefId { get; set; }
            public decimal RequiredAmount { get; set; }
            public string? PaymentLanguage { get; set; }
            public int? MerchantId { get; set; }
            public string? PaymentDestinationId { get; set; }
            public string? Signature { get; set; }
        }

        public class Result
        {
            public int PaymentId { get; set; }
            public string? PaymentUrl { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILoggedUser loggedUser, IVnPayService vnPayService, IConfiguration configuration, ILogger<CreatePayment> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _dbContext = dbContext;
            private readonly ILoggedUser _loggedUser = loggedUser;
            private readonly IVnPayService _vnPayService = vnPayService;
            private readonly IConfiguration _configuration = configuration;
            private readonly ILogger<CreatePayment> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var newPayment = new Domain.Entities.Payment
                    {
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _loggedUser.UserId,
                        PaymentDate = DateTime.UtcNow,
                        ExpireDate = DateTime.UtcNow.AddMinutes(15),
                        PaymentContent = request.PaymentContent,
                        MerchantId = request.MerchantId,
                        PaymentCurrency = request.PaymentCurrency,
                        PaymentDestinationId = request.PaymentDestinationId,
                        PaymentLanguage = request.PaymentLanguage,
                        PaymentRefId = request.PaymentRefId,
                        RequiredAmount = request.RequiredAmount,
                        PaymentSignature = new PaymentSignature
                        {
                            IsValid = true,
                            SignDate = DateTime.UtcNow,
                            SignOwn = request.MerchantId,
                            SignValue = request.Signature
                        }
                    };
                    var createResult = await _dbContext.Payments.AddAsync(newPayment, cancellationToken);
                    var paymentUrl = string.Empty;
                    switch (request.PaymentDestinationId)
                    {
                        case "VnPay":
                            var returnUrl = _configuration["VnPay:ReturnUrl"] ?? string.Empty;
                            var vnPayUrl = _configuration["VnPay:PaymentUrl"] ?? string.Empty;
                            var tmnCode = _configuration["VnPay:TmnCode"] ?? string.Empty;
                            var secretKey = _configuration["VnPay:HashSecret"] ?? string.Empty;
                            var version = _configuration["VnPay:Version"] ?? string.Empty;
                            var createDate = DateTime.UtcNow;
                            var userIpAddress = _loggedUser.IpAddress;
                            var orderType = "other";
                            var paymentCurrency = request.PaymentCurrency ?? "VND";
                            _vnPayService.CreateVnPayRequest(version, tmnCode, createDate, userIpAddress, request.RequiredAmount, paymentCurrency, orderType, request.PaymentContent ?? string.Empty, returnUrl, createResult.Entity.PaymentId.ToString());
                            paymentUrl = _vnPayService.GetLink(vnPayUrl, secretKey);
                            break;
                        default:
                            break;
                    }

                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result
                    {
                        PaymentId = createResult.Entity.PaymentId,
                        PaymentUrl = paymentUrl
                    });
                } catch (Exception ex)
                {
                    _logger.LogError(ex, "CreatePaymentFailure");
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }
        }
    }
}
