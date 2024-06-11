﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Features.Staff;
using Mps.Infrastructure.Middleware;

namespace Mps.Api.Controllers
{
    [Auth(Roles = ["Staff"])]
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPut]
        [Route("shop/status")]
        public async Task<IActionResult> UpdateShopStatus([FromBody] UpdateShopStatus.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }
    }
}