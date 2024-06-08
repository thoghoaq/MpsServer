using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Features.Seller;
using Mps.Infrastructure.Middleware;

namespace Mps.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Auth(Roles = ["ShopOwner"])]
        [HttpGet]
        [Route("shops")]
        public async Task<IActionResult> GetShops([FromQuery] GetShops.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Shops) : BadRequest(result.FailureReason);
        }

        [Auth(Roles = ["ShopOwner"])]
        [HttpPost]
        [Route("shop")]
        public async Task<IActionResult> CreateShop([FromBody] CreateShop.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload?.Message) : BadRequest(new {
                Reason = result.FailureReason
            });
        }
    }
}
