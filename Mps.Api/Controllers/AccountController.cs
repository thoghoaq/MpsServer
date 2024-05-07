using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Features.Account;
using Mps.Infrastructure.Middleware;

namespace Mps.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Auth(Roles = "Admin")]
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _mediator.Send(new GetAllUsers.Query());
            return result.IsSuccess ? Ok(result.Payload?.Users) : BadRequest(result.FailureReason);
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
        [Auth(AllowAnonymous = true)]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] CreateUser.Command command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        /// <remarks>
        /// Example request:
        /// 
        ///     {
        ///         "email": "thonght150201@gmail.com",
        ///         "password": "User@123"
        ///     }
        /// </remarks>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Login.Command command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }
    }
}
