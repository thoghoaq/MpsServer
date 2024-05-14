using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Features.Payment;
using Mps.Infrastructure.Middleware;
using Swashbuckle.AspNetCore.Annotations;

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
        ///         "paymentRefId": 1,
        ///         "requiredAmount": 10000,
        ///         "paymentLanguage": "vn",
        ///         "merchantId": 1,
        ///         "paymentDestinationId": "VnPay",
        ///         "signature": "12345ABCDE"
        ///     }
        /// </remarks>
        /// <param name="command"></param>
        /// <returns></returns>
        [Auth(Roles = ["Customer"])]
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
    }
}
