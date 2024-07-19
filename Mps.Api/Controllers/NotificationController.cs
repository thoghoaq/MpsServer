using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Abstractions.Messaging;
using Mps.Application.Features.Notification;
using Mps.Infrastructure.Middleware;
using System.ComponentModel.DataAnnotations;

namespace Mps.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(INotificationService notificationService, IMediator mediator) : ControllerBase
    {
        private readonly INotificationService _notificationService = notificationService;

        [Auth(Roles = ["SuperAdmin"])]
        [HttpPost("send-all-devices/test")]
        public async Task<IActionResult> SendNotification([FromQuery][Required] int userId, [FromBody] MessageRequest request)
        {
            var result = await _notificationService.SendMessageAllDevicesAsync(userId, request);
            if (result.Any(x => !x.Success))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Auth]
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] GetNotifications.Query query)
        {
            var result = await mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Notifications) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }

        [Auth]
        [HttpPost("read")]
        public async Task<IActionResult> ReadNotification([FromBody] ReadNotification.Command command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }
    }
}
