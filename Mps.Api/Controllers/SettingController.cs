﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Features.Setting;
using Mps.Infrastructure.Middleware;

namespace Mps.Api.Controllers
{
    [Auth(Roles = ["Admin"])]
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetSettings([FromQuery] GetSettings.Query query)
        {
            var result = await mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Settings) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSetting([FromBody] SaveSetting.Command command)
        {
            var result = await mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }
    }
}