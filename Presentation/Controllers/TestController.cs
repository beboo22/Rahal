using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    using Application.Fetures.Authentication.Query.Models;
    using ApplicationBusiness.Fetures.Authentication.Query;
    using ApplicationBusiness.RealTimeservice.NotificationService;
    using ApplicationBusiness.service;
    using Domain.BaseResponce;
    using Domain.Entity.Identity;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class TestNotificationController : ApiController
    {
        private readonly INotificationService _notificationService;

        public TestNotificationController(ISender sender, INotificationService notificationService) : base(sender)
        {

            _notificationService = notificationService;
            _notificationService = notificationService;
        }
        

        [HttpPost("send-test")]
        public async Task<IActionResult> SendTestNotification()
        {
            // بيانات الإشعار الثابتة (Static Data) للتجربة السريعة
            var testNotification = new NotificationDto
            {
                UserId = "49", // حط هنا الـ User ID المفتوح عندك في الشاشة حالياً (سواء 14 أو 1)
                Title = "طلب رحلة جديد! ✈️",
                Body = "قام مستخدم آخر بطلب حجز رحلة معك، اضغط للتفاصيل.",
                Type = NotificationTypes.Booking, // بيقرا من الكلاس الثابت عندك
                ReferenceId = "booking_992",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            // تشغيل السيرفيس لحفظ الإشعار في الـ Redis وبثه عبر الـ SignalR
            await _notificationService.SendAsync(testNotification);

            return Ok(new { Message = "Test notification sent successfully!", Data = testNotification });
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string name)
        {



            if (string.IsNullOrEmpty(name)) return Ok(new List<UserSearchResultDto>());
            var result = await Sender.Send(new GetUserByName(name));

            if(result.statusCode !=200)
                return ProcessResult(result);

            if (result is ApiResultResponse<List<TemplateGenericProfile>> resultResponse)
            {
                // 1. جلب البيانات وتحويلها مع إجبار التنفيذ الفوري باستخدام .ToList()
                var matchedUsersFromDb = resultResponse.Data.Select(x => new UserSearchResultDto
                {
                    Id = x.Id.ToString().Trim(), // تأكيد تحويل الـ ID لنص نظيف بدون مسافات
                    FullName = x.Fname + " " + x.Lname,
                    ProfileImageUrl = x.Traveler != null ? x.Traveler.PhotoUrl :
                                      x.TourGuide != null ? x.TourGuide.PhotoUrl :
                                      x.TravelCompany != null ? x.TravelCompany.PhotoUrl :
                                      "https://cdn-icons-png.flaticon.com/512/149/149071.png",
                    Role = x.Roles.FirstOrDefault() ?? "Traveler"
                }).ToList(); // <-- دي الـمُنقذة اللي هتثبت الداتا في الميموري

                // 2. دلوقتي الـ Loop هيعدل في نفس الأوبجكتس الثابتة في الـ List
                foreach (var user in matchedUsersFromDb)
                {
                    // جلب الاتصال والتأكد من المقارنة بنص نظيف
                    var connectionId = UserConnectionManager.GetConnection(user.Id);
                    user.IsOnline = (connectionId != null);
                }

                return Ok(matchedUsersFromDb);
            }

            return ProcessResult(result);
        }



    }


    public class UserSearchResultDto
    {
        public string Id { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string ProfileImageUrl { get; set; } = default!;
        public string Role { get; set; } = default!;
        public bool IsOnline { get; set; }
    }

}
