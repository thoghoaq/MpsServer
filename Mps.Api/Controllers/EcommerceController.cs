using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Features.Ecommerce;

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
    }
}
