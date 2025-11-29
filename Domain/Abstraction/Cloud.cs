using Domain.BaseResponce;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstraction
{
    /// <summary>
    /// واجهة لخدمات رفع ومعالجة الملفات (مثل الصور والفيديو)
    /// </summary>
    public interface IPhotoService
    {
        /// <summary>
        /// رفع ملف إلى الخدمة السحابية
        /// </summary>
        /// <param name="file">الملف المستلم من الـ Request</param>
        /// <returns>كائن يحتوي على بيانات الملف بعد الرفع (مثل الرابط و الـ ID)</returns>
        Task<ApiResponse> AddPhotoAsync(IFormFile file);

        /// <summary>
        /// حذف ملف من الخدمة السحابية
        /// </summary>
        /// <param name="publicId">الـ ID الفريد للملف (اللي استلمته وقت الرفع)</param>
        /// <param name="resourceType">نوع الملف (مهم جداً: "image", "video", or "raw")</param>
        /// <returns>كائن يحتوي على نتيجة عملية الحذف</returns>
        //Task<ApiResponse> DeletePhotoAsync(string publicId, string resourceType);
    }
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}
