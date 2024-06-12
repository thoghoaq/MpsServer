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

        [Auth(Roles = ["Admin","Staff"])]
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllUsers(string? role, int? pageNumber, int? pageSize, string? query, bool? isActive)
        {
            var result = await _mediator.Send(new GetAllUsers.Query { Filter = query, PageNumber = pageNumber, PageSize = pageSize, Role = role, IsActive = isActive });
            return result.IsSuccess ? Ok(result.Payload?.Users) : BadRequest(result.FailureReason);
        }

        [Auth]
        [HttpGet]
        [Route("logged-user")]
        public async Task<IActionResult> GetLoggedUser()
        {
            var result = await _mediator.Send(new GetUser.Query());
            return result.IsSuccess ? Ok(result.Payload?.User) : BadRequest(result.FailureReason);
        }

        /// <summary>
        /// Register user with role Admin, Staff, Customer and ShopOwner
        /// </summary>
        /// <remarks>
        /// Example request:
        /// 
        ///     {
        ///         "email": "mpsAdmin@gmail.com",
        ///         "password": "User@123",
        ///         "fullName": "Nguyen Van A",
        ///         "role": "Admin"
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

        [Auth]
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUser.Command command)
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
        ///         "email": "mpsAdmin@gmail.com",
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

        [HttpPost]
        [Route("send-password-reset-email")]
        public async Task<IActionResult> SendPasswordResetEmail([FromBody] SendPasswordResetEmail.Command command)
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

        [Auth(Roles = ["SuperAdmin"])]
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUser.Command command)
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
        /// Activate/Deactivate user
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Auth(Roles = ["Admin","Staff"])]
        [HttpPut]
        [Route("status")]
        public async Task<IActionResult> ActiveUser([FromBody] ActiveUser.Command command)
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

        [Auth(Roles = ["Admin"])]
        [HttpGet]
        [Route("staffs/export")]
        public async Task<IActionResult> ExportStaffs([FromQuery] ExportStaffs.Query query)
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
            var fileName = "staffs.xlsx";
            return File(content, contentType, fileName);
        }

        [Auth(Roles = ["Admin"])]
        [HttpPost]
        [Route("staffs/import")]
        public async Task<IActionResult> ImportStaffs([FromForm] ImportStaffs.Command command)
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

        [Auth]
        [HttpGet]
        [Route("details/{userId}")]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            var result = await _mediator.Send(new GetUserDetails.Query { UserId = userId });
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }

        /// <summary>
        /// Get all devices of logged user
        /// </summary>
        /// <returns></returns>
        [Auth]
        [HttpGet]
        [Route("devices")]
        public async Task<IActionResult> GetDevices()
        {
            var result = await _mediator.Send(new GetDevices.Query());
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                reason = result.FailureReason
            });
        }

        [Auth]
        [HttpPut]
        [Route("devices")]
        public async Task<IActionResult> CreateUpdateDevices([FromBody] CreateUpdateDevices.Command command)
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
