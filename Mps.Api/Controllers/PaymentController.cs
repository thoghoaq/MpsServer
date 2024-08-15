using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Abstractions.Payment;
using Mps.Application.Features.Payment;
using Mps.Infrastructure.Dependencies.VnPay.Models;
using Mps.Infrastructure.Middleware;

namespace Mps.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IMediator mediator, IConfiguration configuration, IPayPalService payPalService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Create payment to get link
        /// </summary>
        /// <remarks>
        /// Example request:
        /// 
        ///     {
        ///         "paymentContent": "Thanh toán đơn hàng 001",
        ///         "paymentCurrency": "VND",
        ///         "requiredAmount": 10000,
        ///         "paymentLanguage": "vn",
        ///         "paymentDestinationId": "VnPay",
        ///         "signature": "12345ABCDE",
        ///         "merchants": [
        ///             {
        ///                 "paymentRefId": 1,
        ///                 "merchantId": 1
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        /// <param name="command"></param>
        /// <returns></returns>
        [Auth]
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePayment.Command command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }

        [HttpGet]
        [Route("vnpay-return")]
        public async Task<IActionResult> VnPayReturn([FromQuery] VnPayPayResponse response)
        {
            var processResult = await _mediator.Send(new GetPayment.Query
            {
                Vnp_Amount = response.Vnp_Amount,
                Vnp_BankCode = response.Vnp_BankCode,
                Vnp_BankTranNo = response.Vnp_BankTranNo,
                Vnp_CardType = response.Vnp_CardType,
                Vnp_OrderInfo = response.Vnp_OrderInfo,
                Vnp_TransactionNo = response.Vnp_TransactionNo,
                Vnp_TransactionStatus = response.Vnp_TransactionStatus,
                Vnp_TxnRef = response.Vnp_TxnRef,
                Vnp_SecureHash = response.Vnp_SecureHash,
                Vnp_PayDate = response.Vnp_PayDate,
                Vnp_ResponseCode = response.Vnp_ResponseCode,
                Vnp_TmnCode = response.Vnp_TmnCode
            });

            var webApp = configuration.GetSection("AllowedOrigins:MpsWebApp").Value;
            string url = $"{webApp}?redirect=vnpay-return&success={processResult.IsSuccess}";
            if (processResult.IsSuccess)
            {
                url += $"&paymentId={processResult.Payload?.PaymentId}";
            }
            else
            {
                url += $"&reason={processResult.FailureReason}";
            }
            return Redirect(url);
        }

        [HttpGet]
        [Route("payment-methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var result = await _mediator.Send(new GetPaymentMethods.Query());
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }

        [Auth]
        [HttpGet]
        [Route("details")]
        public async Task<IActionResult> GetPaymentDetails([FromQuery] GetPaymentDetails.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }

        [Auth(Roles = ["Staff"])]
        [HttpPost]
        [Route("accept-payout")]
        public async Task<IActionResult> AcceptPayout([FromBody] AcceptPayout.Command command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }

        [Auth(Roles = ["ShopOwner", "Staff"])]
        [HttpPost]
        [Route("request-payout")]
        public async Task<IActionResult> RequestPayout([FromBody] RequestPayout.Command command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }

        [Auth(Roles = ["Staff"])]
        [HttpPost]
        [Route("request-monthly-payout")]
        public async Task<IActionResult> RequestMonthlyPayout([FromBody] RequestMonthlyPayout.Command command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }

        [Auth(Roles = ["Staff"])]
        [HttpPost]
        [Route("request-weekly-payout")]
        public async Task<IActionResult> RequestWeeklyPayout([FromBody] RequestWeeklyPayout.Command command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }

        [Auth(Roles = ["Staff"])]
        [HttpGet]
        [Route("weekly-payouts")]
        public async Task<IActionResult> GetWeeklyPayouts([FromQuery] GetWeeklyPayouts.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }

        [Auth(Roles = ["Staff"])]
        [HttpPost]
        [Route("custom-request-payout")]
        public async Task<IActionResult> CustomRequestPayout([FromBody] RequestCustomPayout.Command command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }

        [Auth(Roles = ["Admin"])]
        [HttpPost]
        [Route("refund-order")]
        public async Task<IActionResult> RefundOrder([FromBody] RefundOrder.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }
    }
}
