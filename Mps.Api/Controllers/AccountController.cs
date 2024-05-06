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
        [Route("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _mediator.Send(new GetAllUsers.Query());
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(result.FailureReason);
        }
        /// <summary>
        /// Register user with role Admin, User and Supplier
        /// </summary>
        /// <remarks>
        /// Example request:
        /// 
        ///     {
        ///         "email": "thonght150201@gmail.com",
        ///         "password": "User@123",
        ///         "fullName": "Thong Hoang",
        ///         "role": "Customer"
        ///     }
        /// </remarks>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] CreateUser.Command command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }
    }
}
