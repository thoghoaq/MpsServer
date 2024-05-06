using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Features.Account;

namespace Mps.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _mediator.Send(new GetAllUsers.Query());
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(result.FailureReason);
        }
    }
}
