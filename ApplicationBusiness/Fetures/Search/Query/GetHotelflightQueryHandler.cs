using Application.Abstraction.message;
using ApplicationBusiness.Fetures.FlightService.Query;
using ApplicationBusiness.Fetures.FlightService.Query.Model;
using ApplicationBusiness.Fetures.HotelService.Query.Model;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Search.Query
{
    public class HotelFlightResponse
    {
        public List<Hotel> Hotels { get; set; } = new();
        public List<FlightOffer> Flights { get; set; } = new();
    }
    public record GetHotelflight(
    string country,                  // الوجهة المطلوبة (الدولة أو المدينة)
    string? departureAirport,        // كود مطار المغادرة (مثلاً CAI) - مهم للطيران
    DateTime? OutboundDate,            // تاريخ السفر (yyyy-MM-dd)
    DateTime? ReturnDate,              // تاريخ العودة (اختياري في حالة الذهاب فقط)
    DateTime? CheckInDate,             // تاريخ دخول الفندق (yyyy-MM-dd)
    DateTime? CheckOutDate            // تاريخ الخروج من الفندق (yyyy-MM-dd)
    //int Adults = 1,                  // عدد البالغين
    //int Children = 0,                // عدد الأطفال
    //List<int>? ChildrenAges = null   // أعمار الأطفال إن وجدوا
    ) : IQuery<ApiResponse>;
    internal class GetHotelflightQueryHandler : IQueryHandler<GetHotelflight, ApiResponse>
    {
        private ISender sender;

        public GetHotelflightQueryHandler(ISender sender)
        {
            this.sender = sender;
        }

        public async Task<ApiResponse> Handle(GetHotelflight request, CancellationToken cancellationToken)
        {
            // بنجهز اللستات اللي هنملاها بروقان من غير ما نعدل على الـ Result نفسه مباشرة
            List<Hotel> finalHotels = new();
            List<FlightOffer> finalFlights = new();

            // ==================== 1. جلب الفنادق ====================
            var hotelResult = await sender.Send(new GetHotelsspecQuery(new Abstraction.spacification.HotelHistoryFilter
            {
                Destination = request.country
            }), cancellationToken) as ApiResultResponse<List<Hotel>>;

            if (hotelResult?.statusCode == 200 && hotelResult.Data != null)
            {
                finalHotels = hotelResult.Data;
            }
            else // لو 404 أو مفيش داتا في الهيستوري
            {
                var hotelResultserp = await sender.Send(new HotelSearchOrchestratorQuery(new Dtos.Hotels.HotelSearchRequest
                {
                    Destination = request.country,
                    // 🔥 تنبيه: لازم تباصي التواريخ والأفراد هنا لو جايين في الـ request الأساسي
                    CheckInDate = request.CheckInDate?.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    CheckOutDate = request.CheckOutDate?.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd")
                }), cancellationToken) as ApiResultResponse<HotelSearchHistory>;

                if (hotelResultserp?.statusCode == 200 && hotelResultserp.Data?.Hotels != null)
                {
                    finalHotels = hotelResultserp.Data.Hotels.ToList();
                }
            }

            // ==================== 2. جلب الطيران ====================
            var flightResult = await sender.Send(new GetFlightOffer(new Abstraction.spacification.FlightHistoryFilter
            {
                Destination = request.country
            }), cancellationToken) as ApiResultResponse<List<FlightOffer>>;

            if (flightResult?.statusCode == 200 && flightResult.Data != null)
            {
                finalFlights = flightResult.Data;
            }
            else // لو 404 أو مفيش داتا في الهيستوري
            {
                var flightResultserp = await sender.Send(new SearchFlightOrchestratorQuery(new Dtos.Flights.FlightSearchRequest
                {
                    ArrivalId = request.country,
                    // 🔥 تنبيه: لازم تباصي الـ DepartureId والتواريخ هنا عشان الـ Validation ميضربش
                    DepartureId = request.departureAirport ?? "CAI",
                    OutboundDate = request.OutboundDate?.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.ToString("yyyy-MM-dd") //No overload for method 'ToString' takes 1 arguments
                }), cancellationToken) as ApiResultResponse<FlightSearchHistory>;

                if (flightResultserp?.statusCode == 200 && flightResultserp.Data != null)
                {
                    // هنا بنجمع الـ BestFlights والـ OtherFlights بأمان ومن غير ما نقع في فخ الـ void بتاع AddRange
                    var best = flightResultserp.Data.BestFlights ?? Enumerable.Empty<FlightOffer>();
                    var other = flightResultserp.Data.OtherFlights ?? Enumerable.Empty<FlightOffer>();

                    finalFlights = best.Concat(other).ToList();
                }
            }

            // ==================== 3. تجميع الداتا في الـ Response الموحد ====================
            var combinedData = new HotelFlightResponse
            {
                Hotels = finalHotels,
                Flights = finalFlights
            };

            return new ApiResultResponse<HotelFlightResponse>
            (
                200,
                combinedData,
                "Hotels and Flights retrieved successfully."
            );
        }
    }
}
