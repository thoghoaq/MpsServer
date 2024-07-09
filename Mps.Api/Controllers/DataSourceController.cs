using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mps.Application.Features.ProductBrand;
using Mps.Application.Features.ProductCategory;
using Mps.Infrastructure.Middleware;

namespace Mps.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataSourceController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [Route("categories")]
        public async Task<IActionResult> GetCategories([FromQuery] GetCategories.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Categories) : BadRequest(result.FailureReason);
        }

        [Auth(Roles = ["Staff"])]
        [HttpPost]
        [Route("category")]
        public async Task<IActionResult> SaveCategory([FromBody] SaveCategory.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [Auth(Roles = ["Staff"])]
        [HttpDelete]
        [Route("category/{Id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] DeleteCategory.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [HttpGet]
        [Route("brands")]
        public async Task<IActionResult> GetBrands([FromQuery] GetBrands.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Brands) : BadRequest(result.FailureReason);
        }

        [Auth(Roles = ["Staff"])]
        [HttpPost]
        [Route("brand")]
        public async Task<IActionResult> SaveBrand([FromBody] SaveBrand.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [HttpGet]
        [Route("categories/export")]
        public async Task<IActionResult> ExportCategories([FromQuery] ExportCategories.Query query)
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
            var fileName = "product-categories.xlsx";
            return File(content, contentType, fileName);
        }

        [HttpPost]
        [Route("categories/import")]
        public async Task<IActionResult> ImportCategories([FromForm] ImportCategories.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [HttpGet]
        [Route("brands/export")]
        public async Task<IActionResult> ExportBrands([FromQuery] ExportBrands.Query query)
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
            var fileName = "product-brands.xlsx";
            return File(content, contentType, fileName);
        }

        [HttpPost]
        [Route("brands/import")]
        public async Task<IActionResult> ImportBrands([FromForm] ImportBrands.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Payload) : BadRequest(new
            {
                Reason = result.FailureReason
            });
        }

        [HttpGet]
        [Route("models")]
        public async Task<IActionResult> GetModels([FromQuery] GetModels.Query query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Payload?.Models) : BadRequest(result.FailureReason);
        }
    }
}
