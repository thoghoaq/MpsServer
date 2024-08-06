using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Features.Ecommerce;
using Mps.Infrastructure.Middleware;

namespace Mps.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EcommerceController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [Route("products")]
        public async Task<IActionResult> GetAllProducts([FromQuery] GetAllProducts.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Products) : BadRequest(result.FailureReason);
        }

        [HttpGet]
        [Route("products/{Id}")]
        public async Task<IActionResult> GetProductById([FromRoute] GetProductDetails.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Product) : BadRequest(result.FailureReason);
        }

        [Auth(Roles = ["Customer"])]
        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> Checkout([FromBody] Checkout.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [Auth(Roles = ["Customer"])]
        [HttpGet]
        [Route("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] GetOrders.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Orders) : BadRequest(result.FailureReason);
        }

        [HttpPost]
        [Route("tracking-product")]
        public async Task<IActionResult> TrackingProduct([FromBody] TrackingProduct.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [Auth(Roles = ["Customer"])]
        [HttpPost]
        [Route("feedback")]
        public async Task<IActionResult> Feedback([FromBody] Feedback.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [HttpGet]
        [Route("feedbacks")]
        public async Task<IActionResult> GetFeedbacks([FromQuery] GetFeedbacks.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(result.FailureReason);
        }

        [HttpGet]
        [Route("similar")]
        public async Task<IActionResult> GetSimilarProducts([FromQuery] GetSimilarProducts.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Products) : BadRequest(result.FailureReason);
        }

        [HttpGet]
        [Route("shop-rating")]
        public async Task<IActionResult> GetShopRating([FromQuery] GetShopRating.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(result.FailureReason);
        }
    }
}
