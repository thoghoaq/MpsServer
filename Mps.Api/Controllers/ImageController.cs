using Microsoft.AspNetCore.Mvc;
using Mps.Application.Abstractions.Storage;
using Mps.Domain.Enums;
using Mps.Domain.Extensions;
using Mps.Infrastructure.Middleware;
using System.ComponentModel.DataAnnotations;

namespace Mps.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController(IStorageService storageService) : ControllerBase
    {
        private readonly IStorageService _storageService = storageService;

        /// <summary>
        /// Post image to firebase storage and return the url
        /// </summary>
        /// <param name="type">UserAvatars, ProductImages, General</param>
        /// <param name="file">Only accept png, jpg and jpeg</param>
        /// <returns></returns>
        [Auth(Roles = ["SuperAdmin"])]
        [HttpPost]
        [Route("upload/test")]
        public async Task<IActionResult> Post([Required]IFormFile file, [Required]ImageType type)
        {
            try
            {
                var folderPath = $"/{type.GetDescription()}";
                var fileUrl = await _storageService.UploadImageAsync(file, folderPath);
                return Ok(new
                {
                    fileUrl
                });
            } catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadImages([Required][FromForm] List<IFormFile> files, [Required][FromQuery] ImageType type)
        {
            try
            {
                var folderPath = $"/{type.GetDescription()}";
                var fileUrls = await _storageService.UploadMultipleImagesAsync(files, folderPath);
                return Ok(fileUrls);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete]
        [Route("delete/test")]
        public async Task<IActionResult> Delete([Required]string imageUrl)
        {
            try
            {
                await _storageService.DeleteImageAsync(imageUrl);
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
