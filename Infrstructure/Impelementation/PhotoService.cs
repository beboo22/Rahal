using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Domain.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BaseResponce;

namespace Infrstructure.Impelementation
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        /// <summary>
        /// الكونستركتور بيعمل حقن (Inject) للإعدادات 
        /// و بينشئ الـ Cloudinary client
        /// </summary>
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        /// <summary>
        /// تنفيذ عملية رفع الملف
        /// </summary>
        public async Task<ApiResponse> AddPhotoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new ApiResponse(400);
            }

            // استخدام AutoUploadParams بيخلي Cloudinary يحدد نوع الملف أوتوماتيك
            // (image, video, or raw for files like PDF/DOCX)
            var uploadParams = new AutoUploadParams
            {
                // استخدام ستريم آمن
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                // (اختياري) اسم الفولدر اللي هيتخزن فيه على Cloudinary
                Folder = "rahal_uploads"
            };

            try
            {
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                // [!!!] التعديل الأول: اتأكد إن الرد مفيهوش خطأ صريح
                if (uploadResult.Error != null)
                {
                    return new ApiResponse(500, uploadResult.Error.Message);
                }

                // [!!!] التعديل الثاني (الأهم): اتأكد إن الـ HTTP Status هو "OK"
                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    // لو الـ Status مش OK (زي 401 أو 400)، رجع رسالة الخطأ
                    string errorMessage = uploadResult.Error.Message ?? $"Upload failed with status code: {uploadResult.StatusCode}";
                    return new ApiResponse((int)uploadResult.StatusCode, errorMessage);
                }

                // رجع البيانات المهمة دي علشان تسجلها في الداتابيز عندك

                return new ApiResponse(200, uploadResult.SecureUrl.ToString());
            }
            catch (Exception ex)
            {
                return new ApiResponse(500,ex.Message);
            }
        }

        /// <summary>
        /// تنفيذ عملية حذف الملف
        /// </summary>
        //public async Task<ApiResponse> DeletePhotoAsync(string publicId, string resourceType)
        //{
        //    if (string.IsNullOrEmpty(publicId) || string.IsNullOrEmpty(resourceType))
        //    {
        //        return new DeletionResult
        //        {
        //            IsSuccess = false,
        //            ErrorMessage = "PublicId or ResourceType is missing."
        //        };
        //    }

        //    try
        //    {
        //        // تحويل النوع من string إلى Enum اللي Cloudinary بيفهمه
        //        var resourceTypeEnum = (ResourceType)Enum.Parse(typeof(ResourceType), resourceType, true);

        //        var deletionParams = new DeletionParams(publicId)
        //        {
        //            ResourceType = resourceTypeEnum
        //        };

        //        var result = await _cloudinary.DestroyAsync(deletionParams);

        //        // "ok" معناها اتمسح بنجاح
        //        if (result.Result?.ToLower() == "ok")
        //        {
        //            return new DeletionResult { IsSuccess = true };
        //        }

        //        // "not found" معناها إنه ملقاش الملف بالـ ID ده
        //        return new DeletionResult
        //        {
        //            IsSuccess = false,
        //            ErrorMessage = result.Result
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DeletionResult
        //        {
        //            IsSuccess = false,
        //            ErrorMessage = ex.Message
        //        };
        //    }
        //}
   
    }
}
