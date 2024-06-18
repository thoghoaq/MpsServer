using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Features.Payment;
using Mps.Infrastructure.Dependencies.VnPay.Models;
using Mps.Infrastructure.Middleware;

namespace Mps.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IMediator mediator) : ControllerBase
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
            return processResult.IsSuccess ? Ok(processResult.Payload) : BadRequest(new
            {
                reason = processResult.FailureReason
            });
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
    }
}
