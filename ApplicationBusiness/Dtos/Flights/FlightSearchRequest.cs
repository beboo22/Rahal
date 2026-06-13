using Domain.Entity.Hotel_flights;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Flights
{
    public class FlightSearchRequest
    {
        /// <summary>Origin airport IATA code (e.g. "CAI", "JFK")</summary>
        [Required]
        [StringLength(10)]
        public string DepartureId { get; set; } = string.Empty;

        /// <summary>Destination airport IATA code</summary>
        [Required]
        [StringLength(10)]
        public string ArrivalId { get; set; } = string.Empty;

        /// <summary>Departure date in yyyy-MM-dd format</summary>
        [Required]
        public string OutboundDate { get; set; } = string.Empty;

        /// <summary>Return date in yyyy-MM-dd format (required for round-trip)</summary>
        public string? ReturnDate { get; set; }

        [Range(1, 9)]
        public int Adults { get; set; } = 1;

        [Range(0, 8)]
        public int Children { get; set; } = 0;

        public TravelClass TravelClass { get; set; } = TravelClass.Economy;

        public TripType TripType { get; set; } = TripType.RoundTrip;

        /// <summary>ISO 4217 currency code</summary>
        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        /// <summary>Country code (e.g. "us", "eg")</summary>
        [StringLength(5)]
        public string Gl { get; set; } = "us";

        /// <summary>Language code (e.g. "en", "ar")</summary>
        [StringLength(5)]
        public string Hl { get; set; } = "en";
        public List<int> ChildrenAges { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
   
    public class FlightSearchResponse
    {
        public string currency { get; set; }

        public List<FlightResult> BestFlights { get; set; } = new();
        public List<FlightResult> OtherFlights { get; set; } = new();
        public PriceInsights? PriceInsights { get; set; }
        public string SearchId { get; set; } = string.Empty;
    }

    public class FlightResult
    {
        public List<FlightLeg> Flights { get; set; } = new();
        public int TotalDuration { get; set; }
        public int Price { get; set; }
        public string Type { get; set; } = string.Empty;
        public List<string> AirlineLogo { get; set; } = new();
        public int Layovers { get; set; }
        public bool CarbonEmissions { get; set; }
        public string BookingToken { get; set; } = string.Empty;
    }

    public class FlightLeg
    {
        public Airport DepartureAirport { get; set; } = new();
        public Airport ArrivalAirport { get; set; } = new();
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Airplane { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string AirlineLogo { get; set; } = string.Empty;
        public string TravelClass { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public bool Overnight { get; set; }
        public int? LegRoom { get; set; }
    }

    public class Airport
    {
        public string Name { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
    }

    public class PriceInsights
    {
        public int LowestPrice { get; set; }
        public string PriceLevel { get; set; } = string.Empty;
        public List<int[]> PriceHistory { get; set; } = new();
        public PriceInsights()
        {

        }
        public PriceInsights(int lowestPrice, string priceLevel)
        {
            LowestPrice = lowestPrice;
            PriceLevel = priceLevel;
        }

    }



    public static class FlightMappingExtensions
    {
        /// <summary>
        /// Expression Mapping مطور ومبني بالكامل على الـ Domain والـ DTOs الخاصة بك حرفياً.
        /// </summary>
        public static Expression<Func<PayFlight, FlightResult>> MapToFlightResult => booking => new FlightResult
        {
            // 1. تحويل السعر من decimal إلى int ليناسب الـ DTO
            Price = (int)booking.TotalBookingPrice,

            // 2. جلب الخصائص الأساسية من الـ FlightOffer التابع لك
            TotalDuration = booking.FlightOffer != null ? booking.FlightOffer.TotalDuration : 0,
            Type = booking.FlightOffer != null ? booking.FlightOffer.Type : string.Empty,
            Layovers = booking.FlightOffer != null ? booking.FlightOffer.Layovers : 0,
            CarbonEmissions = booking.FlightOffer != null && booking.FlightOffer.CarbonEmissions,
            BookingToken = booking.FlightOffer != null ? booking.FlightOffer.BookingToken : string.Empty,

            // بما أن الـ Domain لا يحتوي على خاصية للـ Logos، نتركها مصفوفة فارغة مؤقتاً لتطابق الـ DTO
            AirlineLogo = new List<string>(),

            // 3. عمل مابينج لقائمة الـ Flights (التي نوعها FlightSegment في الـ Domain ونحولها لـ FlightLeg في الـ DTO)
            Flights = booking.FlightOffer != null
                ? booking.FlightOffer.Flights.Select(leg => new FlightLeg
                {
                    Duration = leg.Duration,
                    Airplane = leg.Airplane,
                    Airline = leg.Airline,
                    AirlineLogo = leg.AirlineLogo,
                    TravelClass = leg.TravelClass,
                    FlightNumber = leg.FlightNumber,
                    Overnight = leg.Overnight,
                    LegRoom = leg.LegRoom, // تحويل تلقائي من int إلى int?

                    // تحويل الـ DateTime الموجود في الـ Domain عندك إلى string متوافق مع صيغة الـ DTO
                    DepartureTime = leg.DepartureTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ArrivalTime = leg.ArrivalTime.ToString("yyyy-MM-ddTHH:mm:ss"),

                    // مابينج مطار المغادرة (من AirportInfo في الـ Domain إلى Airport في الـ DTO)
                    DepartureAirport = leg.DepartureAirport != null ? new Airport
                    {
                        Id = leg.DepartureAirport.Code, // جلب الـ Code من الـ Domain لوضعه في الـ Id الخاص بالـ DTO
                        Name = leg.DepartureAirport.Name,
                        Time = leg.DepartureAirport.Time.ToString("yyyy-MM-ddTHH:mm:ss") // تحويل الـ DateTime الخاص بالوقت إلى string
                    } : new Airport(),

                    // مابينج مطار الوصول (من AirportInfo في الـ Domain إلى Airport في الـ DTO)
                    ArrivalAirport = leg.ArrivalAirport != null ? new Airport
                    {
                        Id = leg.ArrivalAirport.Code,
                        Name = leg.ArrivalAirport.Name,
                        Time = leg.ArrivalAirport.Time.ToString("yyyy-MM-ddTHH:mm:ss")
                    } : new Airport()
                }).ToList()
                : new List<FlightLeg>()
        };
    }
}
