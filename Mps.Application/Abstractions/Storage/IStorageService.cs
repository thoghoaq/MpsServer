using Microsoft.AspNetCore.Http;

namespace Mps.Application.Abstractions.Storage
{
    public interface IStorageService
    {
        public Task<string> UploadImageAsync(IFormFile file, string folderPath);
        public Task DeleteImageAsync(string imageUrl);
    }
}
