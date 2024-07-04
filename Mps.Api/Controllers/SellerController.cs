using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Features.Seller;
using Mps.Infrastructure.Middleware;

namespace Mps.Api.Controllers
{
    [Auth(Roles = ["ShopOwner"])]
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [Route("shops")]
        public async Task<IActionResult> GetShops([FromQuery] GetShops.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Shops) : BadRequest(result.FailureReason);
        }

        [HttpPost]
        [Route("shop")]
        public async Task<IActionResult> CreateShop([FromBody] CreateShop.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [HttpPut]
        [Route("shop/{Id}")]
        public async Task<IActionResult> UpdateShop([FromRoute] int Id, [FromBody] UpdateShop.Command command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [HttpGet]
        [Route("shop/{Id}")]
        public async Task<IActionResult> GetShopDetail([FromRoute] GetShopDetail.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [HttpGet]
        [Route("shop-overview")]
        public async Task<IActionResult> GetShopOverview([FromQuery] GetShopOverview.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }
    }
}
