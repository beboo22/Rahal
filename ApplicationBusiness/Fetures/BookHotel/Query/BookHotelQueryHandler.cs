using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Dtos.Hotels;
using ApplicationBusiness.Fetures.BookHotel.Query.Models;
using ApplicationBusiness.Fetures.BookingTripService.Query.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BookHotel.Query
{
    internal class BookHotelQueryHandler : IQueryHandler<GetHotelBooking, ApiResponse>
    {
        private readonly IReadGenericRepo<PayHotel> _repo;

        public BookHotelQueryHandler(IReadGenericRepo<PayHotel> repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse> Handle(GetHotelBooking request, CancellationToken cancellationToken)
        {
            // 1. استخدام الـ Spec الخاص بك مع تفعيل الـ Includes لـ Hotel و User
            var spec = new PayHotelSpecification(request.Filter).IncludeHotelAndUser();

            // 2. جلب الحجوزات (PayHotel) مباشرة من قاعدة البيانات بكامل خصائصها المالية
            var bookings = await _repo.GetAllSpec(spec).ToListAsync(cancellationToken);

            // 3. إذا لم نجد أي حجوزات تطابق الفلتر، نرجع خطأ 404
            if (!bookings.Any())
                return new ApiResponse(404, "No hotel bookings found.");

            // 4. إذا كان الـ Client يبحث عن حجز مالي محدد بواسطة الـ ID، نرجع كائن الحجز (PayHotel) المفرد
            if (request.Filter.Id.HasValue)
            {
                return new ApiResultResponse<PayHotel>(200, bookings.First());
            }

            // 5. في حالة جلب كافة الحجوزات (مثلاً: قائمة حجوزات مستخدم معين)، نرجع قائمة كاملة من الـ List<PayHotel>
            return new ApiResultResponse<List<PayHotel>>(200, bookings);
        }
    }
}
