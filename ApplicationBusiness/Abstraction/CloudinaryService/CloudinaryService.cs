using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ApplicationBusiness.Abstraction.CloudinaryService
{
    public interface ICloudinaryService
    {
        public Task<string?> UploadFileAsync(IFormFile file);
    }

    public class CloudinaryService: ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:APIKey"],
                configuration["Cloudinary:APISecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string?> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            await using var stream = file.OpenReadStream();

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (IsVideo(extension))
            {
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "uploads/videos"
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                return result?.SecureUrl?.ToString();
            }
            else
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "uploads/images"
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                return result?.SecureUrl?.ToString();
            }
        }

        private bool IsVideo(string extension)
        {
            string[] videoExtensions =
            {
            ".mp4", ".mov", ".avi", ".wmv", ".mkv", ".webm"
        };

            return videoExtensions.Contains(extension);
        }
    }
}
