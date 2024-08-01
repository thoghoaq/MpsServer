using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Features.Shop;
using Mps.Infrastructure.Middleware;

namespace Mps.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Auth(Roles = ["Staff"])]
        [HttpGet]
        public async Task<IActionResult> GetShops([FromQuery] GetShops.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Shops) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [Auth(Roles = ["Staff"])]
        [HttpGet]
        [Route("request")]
        public async Task<IActionResult> GetShopRequest([FromQuery] GetShopRequest.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Shops) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [Auth(Roles = ["Staff"])]
        [HttpPost]
        [Route("accept-request")]
        public async Task<IActionResult> AcceptShopRequest([FromBody] AcceptShopRequest.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [Auth(Roles = ["ShopOwner"])]
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

        [Auth(Roles = ["ShopOwner"])]
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

        [Auth(Roles = ["ShopOwner"])]
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

        [Auth(Roles = ["ShopOwner"])]
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

        [Auth(Roles = ["ShopOwner"])]
        [HttpPut]
        [Route("product/status")]
        public async Task<IActionResult> DeactiveProduct([FromBody] DeactiveProduct.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [Auth(Roles = ["ShopOwner"])]
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

        [Auth(Roles = ["ShopOwner", "Staff"])]
        [HttpPost]
        [Route("order/status")]
        public async Task<IActionResult> ChangeOrderStatus([FromBody] ChangeOrderStatus.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [Auth(Roles = ["ShopOwner", "Staff"])]
        [HttpGet]
        [Route("revenue")]
        public async Task<IActionResult> GetRevenue([FromQuery] GetRevenue.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [Auth(Roles = ["ShopOwner", "Staff"])]
        [HttpGet]
        [Route("orders-in-payout-date")]
        public async Task<IActionResult> GetOrdersInPayoutDate([FromQuery] GetOrdersInPayoutDate.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Orders) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }
    }
}
