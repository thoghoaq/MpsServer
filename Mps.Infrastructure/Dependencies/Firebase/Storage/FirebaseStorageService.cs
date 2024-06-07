using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Storage;

namespace Mps.Infrastructure.Dependencies.Firebase.Storage
{
    public class FirebaseStorageService(IConfiguration configuration, ILogger<FirebaseStorageService> logger) : IStorageService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<FirebaseStorageService> _logger = logger;

        public async Task<string> UploadImageAsync(IFormFile file, string folderPath)
        {
            try
            {
                if (file.ContentType != "image/png" && file.ContentType != "image/jpeg" && file.ContentType != "image/jpg")
                {
                    throw new Exception("Invalid file type. Only PNG, JPG and JPEG are allowed.");
                }
                var bucket = _configuration["FirebaseStorage:Bucket"];
                var folders = folderPath.Split("/").Where(x => !string.IsNullOrEmpty(x)).ToArray();
                var fileName = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + file.FileName;

                var task = new FirebaseStorage(bucket, new FirebaseStorageOptions
                {
                    ThrowOnCancel = true
                }).Child("Images");

                task = folders.Aggregate(task, (current, folder) => current.Child(folder));
                string imageUrl = await task.Child(fileName).PutAsync(file.OpenReadStream());
                return imageUrl;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "FirebaseStorageServiceFailure");
                throw;
            }
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            try
            {
                var bucket = _configuration["FirebaseStorage:Bucket"];
                var task = new FirebaseStorage(bucket, new FirebaseStorageOptions
                {
                    ThrowOnCancel = true
                }).Child("Images");
                Uri uri = new(imageUrl);
                string blobName = Uri.UnescapeDataString(uri.AbsolutePath[(uri.AbsolutePath.IndexOf("/o/") + 3)..]);
                var folders = blobName[7..].Split("/").Where(x => !string.IsNullOrEmpty(x)).ToArray();
                task = folders.Aggregate(task, (current, folder) => current.Child(folder));
                await task.DeleteAsync();
            } catch (Exception ex)
            {
                _logger.LogError(ex, "FirebaseStorageServiceFailure");
                throw new Exception("Error deleting image from Firebase Storage", ex);
            }
        }

        public async Task<List<string>> UploadMultipleImagesAsync(List<IFormFile> files, string folderPath)
        {
            if (files.Any(x => x.ContentType != "image/png" && x.ContentType != "image/jpeg" && x.ContentType != "image/jpg"))
            {
                throw new Exception("Invalid file type. Only PNG, JPG and JPEG are allowed.");
            }
            var fileUrls = new List<string>();
            foreach (var file in files)
            {
                var url = await UploadImageAsync(file, folderPath);
                fileUrls.Add(url);
            }
            return fileUrls;
        }
    }
}
