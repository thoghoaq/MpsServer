using Microsoft.AspNetCore.Mvc;
using Mps.Application.Abstractions.Messaging;
using Mps.Infrastructure.Middleware;
using System.ComponentModel.DataAnnotations;

namespace Mps.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(INotificationService notificationService) : ControllerBase
    {
        private readonly INotificationService _notificationService = notificationService;

        [Auth(Roles = ["SuperAdmin"])]
        [HttpPost("send-all-devices/test")]
        public async Task<IActionResult> SendNotification([FromQuery][Required] int userId, [FromBody] MessageRequest request)
        {
            var result = await _notificationService.SendMessageAllDevicesAsync(userId ,request);
            if (result.Any(x => !x.Success))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
