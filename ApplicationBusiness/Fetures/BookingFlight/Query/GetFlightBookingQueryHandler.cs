using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Dtos.Flights;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PriceInsights = ApplicationBusiness.Dtos.Flights.PriceInsights;

namespace ApplicationBusiness.Fetures.BookingFlight.Query
{
    public record GetFlightBookingQuery(PaymentFilter Filter) : IQuery<ApiResponse>;
    internal class GetFlightBookingQueryHandler : IQueryHandler<GetFlightBookingQuery, ApiResponse>
    {
        private readonly IReadGenericRepo<PayFlight> _repo;

        public GetFlightBookingQueryHandler(IReadGenericRepo<PayFlight> repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse> Handle(GetFlightBookingQuery request, CancellationToken cancellationToken)
        {
            // 1. استخدام الـ Spec الخاص بالطيران لتفعيل الـ Includes للـ FlightOffer والـ Flights والـ User
            var spec = new PayFlightSpecification(request.Filter).IncludeFlightOfferAndUser();

            // 2. جلب الحجوزات (PayFlight) مباشرة بكامل تفاصيلها المالية من قاعدة البيانات
            var bookings = await _repo.GetAllSpec(spec).ToListAsync(cancellationToken);

            // 3. إذا لم نجد نتائج تطابق الفلتر نرجع 404
            if (!bookings.Any())
                return new ApiResponse(404, "No flight bookings found.");

            // 4. إذا كان الـ Client يبحث عن حجز مالي محدد بـ ID، نرجع كائن الحجز (PayFlight) بالكامل
            if (request.Filter.Id.HasValue)
            {
                return new ApiResultResponse<PayFlight>(200, bookings.First());
            }

            // 5. في حالة جلب كافة الحجوزات (مثل قائمة حجوزات الطيران الخاصة بمستخدم)، نرجع قائمة الـ List<PayFlight> كاملة
            return new ApiResultResponse<List<PayFlight>>(200, bookings);
        }
    }
}
