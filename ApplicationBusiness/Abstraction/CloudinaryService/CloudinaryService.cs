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
        Task<string?> UploadFileAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string fileUrl);
    }
    public class CloudinaryService : ICloudinaryService
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
            if (file == null || file.Length == 0) return null;

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

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            // 1. لو فاضي أصلاً ارجع true (على أساس إن مفيش ملف يتمسح فكده تمام)
            if (string.IsNullOrEmpty(fileUrl)) return true;

            // 2. التحقق هل هو رابط صالح وهل يخص Cloudinary
            // لو مش لينك (مجرد text) أو لينك ملوش علاقة بـ Cloudinary هيرجع true ويفكس
            if (!Uri.TryCreate(fileUrl, UriKind.Absolute, out _) || !fileUrl.Contains("cloudinary.com"))
            {
                return true;
            }

            try
            {
                // Extract Public ID from URL
                var publicId = ExtractPublicId(fileUrl);

                // Determine if it's a video or image
                var resourceType = fileUrl.Contains("/video/") ? ResourceType.Video : ResourceType.Image;

                var deleteParams = new DeletionParams(publicId)
                {
                    ResourceType = resourceType
                };

                var result = await _cloudinary.DestroyAsync(deleteParams);

                // لو النتيجة ok أو حتى لو الصورة مش موجودة أصلاً (not found) بنرجع true
                return result.Result == "ok" || result.Result == "not found";
            }
            catch
            {
                // في حالة حصل مشكلة في الـ API نفسه ممكن ترجع true عشان متوقفش الـ delete بتاع الـ Record من الداتا بيز
                return true;
            }
        }

        private string ExtractPublicId(string url)
        {
            // Example URL: https://res.cloudinary.com/demo/image/upload/v12345/uploads/images/sample.jpg
            // Public ID should be: uploads/images/sample
            var uri = new Uri(url);
            var pathSegments = uri.AbsolutePath.Split('/');

            // The public ID is usually everything after the version (v12345) 
            // and before the file extension.
            var startIndex = Array.FindIndex(pathSegments, s => s.StartsWith("v") && s.Length > 1 && char.IsDigit(s[1])) + 1;

            // If no versioning found, fallback to standard segments
            if (startIndex == 0) startIndex = 4;

            var publicIdWithExtension = string.Join("/", pathSegments.Skip(startIndex));
            return Path.ChangeExtension(publicIdWithExtension, null); // Removes .jpg, .png, etc.
        }

        private bool IsVideo(string extension)
        {
            string[] videoExtensions = { ".mp4", ".mov", ".avi", ".wmv", ".mkv", ".webm" };
            return videoExtensions.Contains(extension);
        }
    }
}
