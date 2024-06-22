using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Features.Shop;
using Mps.Infrastructure.Middleware;

namespace Mps.Api.Controllers
{
    [Auth(Roles = ["ShopOwner"])]
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [Route("products")]
        public async Task<IActionResult> GetProducts([FromQuery] GetProducts.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Products) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [HttpGet]
        [Route("product")]
        public async Task<IActionResult> GetProduct([FromQuery] GetProduct.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [HttpPost]
        [Route("product")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProduct.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [HttpPut]
        [Route("product")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProduct.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [HttpGet]
        [Route("products/export")]
        public async Task<IActionResult> ExportProducts([FromQuery] ExportProducts.Query query)
        {
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    Reason = result.FailureReason
                });
            }
            var stream = result.Payload?.FileStream;
            var content = stream?.ToArray() ?? [];
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "products.xlsx";
            return File(content, contentType, fileName);
        }

        [Auth(Roles = ["ShopOwner", "Staff"])]
        [HttpGet]
        [Route("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] GetOrders.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Orders) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }
    }
}
