using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Payment;
using Mps.Application.Commons;
using Mps.Application.Helpers;
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
            public int MerchantId { get; set; }
            public string? PaymentDestinationId { get; set; }
        }

        public class Result
        {
            public int PaymentId { get; set; }
            public string? PaymentUrl { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILoggedUser loggedUser, IVnPayService vnPayService, IConfiguration configuration, ILogger<CreatePayment> logger, IHttpContextAccessor httpContext) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _dbContext = dbContext;
            private readonly ILoggedUser _loggedUser = loggedUser;
            private readonly IVnPayService _vnPayService = vnPayService;
            private readonly IConfiguration _configuration = configuration;
            private readonly ILogger<CreatePayment> _logger = logger;
            private readonly IHttpContextAccessor _httpContext = httpContext;

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
                        Content = request.PaymentContent,
                        MerchantId = request.MerchantId,
                        Currency = request.PaymentCurrency,
                        PaymentDestinationId = request.PaymentDestinationId,
                        Language = request.PaymentLanguage,
                        RefId = request.PaymentRefId,
                        RequiredAmount = request.RequiredAmount,
                        PaymentStatusId = (int)Domain.Enums.PaymentStatus.Pending,
                        PaymentSignature = new PaymentSignature
                        {
                            IsValid = true,
                            SignDate = DateTime.UtcNow,
                            SignOwn = request.MerchantId,
                            SignValue = GenerateSignature(request.MerchantId.ToString(), request.PaymentContent ?? string.Empty, request.PaymentCurrency ?? string.Empty, request.PaymentDestinationId ?? string.Empty, request.PaymentLanguage ?? string.Empty, request.PaymentRefId ?? 0, request.RequiredAmount, _configuration["VnPay:HashSecret"] ?? string.Empty)
                        }
                    };
                    var createResult = await _dbContext.Payments.AddAsync(newPayment, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    var paymentUrl = string.Empty;
                    switch (request.PaymentDestinationId)
                    {
                        case "VnPay":
                            var appDomain = $"{_httpContext.HttpContext?.Request.Scheme}://{_httpContext.HttpContext?.Request.Host}";
                            var returnUrl = $"{appDomain}{_configuration["VnPay:ReturnUrl"]}";
                            var vnPayUrl = _configuration["VnPay:PaymentUrl"] ?? string.Empty;
                            var tmnCode = _configuration["VnPay:TmnCode"] ?? string.Empty;
                            var secretKey = _configuration["VnPay:HashSecret"] ?? string.Empty;
                            var version = _configuration["VnPay:Version"] ?? string.Empty;
                            var createDate = DateTime.UtcNow;
                            var userIpAddress = _loggedUser.IpAddress;
                            var orderType = "other";
                            var paymentCurrency = request.PaymentCurrency ?? "VND";
                            _vnPayService.CreateVnPayRequest(version, tmnCode, createDate, userIpAddress, request.RequiredAmount, paymentCurrency, orderType, request.PaymentContent ?? string.Empty, returnUrl, createResult.Entity.Id.ToString());
                            paymentUrl = _vnPayService.GetLink(vnPayUrl, secretKey);
                            break;
                        default:
                            break;
                    }
                    return CommandResult<Result>.Success(new Result
                    {
                        PaymentId = createResult.Entity.Id,
                        PaymentUrl = paymentUrl
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CreatePaymentFailure");
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }

            private string GenerateSignature(string merchantId, string paymentContent, string paymentCurrency, string paymentDestinationId, string paymentLanguage, int paymentRefId, decimal requiredAmount, string secretKey)
            {
                var data = $"{merchantId}{paymentContent}{paymentCurrency}{paymentDestinationId}{paymentLanguage}{paymentRefId}{requiredAmount}";
                return secretKey.HmacSHA512(data);
            }
        }
    }
}
