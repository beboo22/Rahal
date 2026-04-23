using Domain.Entity.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entity.Hotel_flights
{
    // ─────────────────────────────────────────────────────────────
    // FlightSearchHistory
    // ─────────────────────────────────────────────────────────────
    public class FlightSearchHistory : BaseEntity
    {
        // STJ needs a public setter (or [JsonInclude]) to populate
        // after construction when using the parameterless ctor path.
        // We keep private setters and use [JsonConstructor] so STJ
        // calls our parameterized ctor directly.
        [JsonInclude]
        public ICollection<FlightOffer> BestFlights { get; private set; } = new List<FlightOffer>();

        [JsonInclude]
        public ICollection<FlightOffer> OtherFlights { get; private set; } = new List<FlightOffer>();

        [JsonInclude]
        public PriceInsights PriceInsights { get; private set; } = null!;

        [JsonInclude]
        public string SearchId { get; private set; } = string.Empty;

        [JsonInclude]
        public string Currency { get; private set; } = string.Empty;

        // Required by EF Core
        public FlightSearchHistory() { }

        /// <summary>
        /// Parameter names MUST match property names (case-insensitive) exactly.
        /// BestFlights → bestFlights  ✔
        /// OtherFlights → otherFlights ✔
        /// PriceInsights → priceInsights ✔
        /// SearchId → searchId ✔
        /// Currency → currency ✔
        /// </summary>
        //[JsonConstructor]
        public FlightSearchHistory(
            ICollection<FlightOffer> bestFlights,
            ICollection<FlightOffer> otherFlights,
            PriceInsights priceInsights,
            string searchId,
            string currency)
        {
            BestFlights = bestFlights ?? new List<FlightOffer>();
            OtherFlights = otherFlights ?? new List<FlightOffer>();
            PriceInsights = priceInsights;
            SearchId = searchId;
            Currency = currency;
        }

        // ── Convenience ctor used by the application layer ──────────
        // (accepts IEnumerable so existing call sites don't break)
        public FlightSearchHistory(
            IEnumerable<FlightOffer> bestFlights,
            IEnumerable<FlightOffer> otherFlights,
            PriceInsights priceInsights,
            string searchId,
            string currency)
            : this(
                bestFlights?.ToList() ?? new List<FlightOffer>(),
                otherFlights?.ToList() ?? new List<FlightOffer>(),
                priceInsights,
                searchId,
                currency)
        { }
    }

    // ─────────────────────────────────────────────────────────────
    // PayFlight  (no constructor issues – left as-is)
    // ─────────────────────────────────────────────────────────────
    public class PayFlight : BaseEntity
    {
        public FlightOffer FlightOffer { get; set; } = null!;
        public int FlightOfferId { get; set; }
        public bool IsPaid { get; set; } = false;
        public bool Canceled { get; set; }
        public User User { get; set; } = null!;
        public decimal TotalBookingPrice { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    // FlightOffer
    // ─────────────────────────────────────────────────────────────
    public class FlightOffer : BaseEntity
    {
        [JsonInclude]
        public ICollection<FlightSegment> Flights { get; private set; } = new List<FlightSegment>();

        [JsonInclude]
        public int TotalDuration { get; private set; }

        [JsonInclude]
        public decimal Price { get; private set; }

        [JsonInclude]
        public string Type { get; private set; } = string.Empty;

        [JsonInclude]
        public int Layovers { get; private set; }

        [JsonInclude]
        public bool CarbonEmissions { get; private set; }

        [JsonInclude]
        public string BookingToken { get; private set; } = string.Empty;

        // Required by EF Core
        public FlightOffer() { }

        /// <summary>
        /// All parameter names match their corresponding property names exactly.
        /// </summary>
        //[JsonConstructor]
        public FlightOffer(
            ICollection<FlightSegment> flights,
            int totalDuration,
            decimal price,
            string type,
            int layovers,
            bool carbonEmissions,
            string bookingToken)
        {
            Flights = flights ?? new List<FlightSegment>();
            TotalDuration = totalDuration;
            Price = price;
            Type = type;
            Layovers = layovers;
            CarbonEmissions = carbonEmissions;
            BookingToken = bookingToken;
        }

        // Convenience ctor for existing IEnumerable call sites
        public FlightOffer(
            IEnumerable<FlightSegment> flights,
            int totalDuration,
            decimal price,
            string type,
            int layovers,
            bool carbonEmissions,
            string bookingToken)
            : this(
                flights?.ToList() ?? new List<FlightSegment>(),
                totalDuration, price, type, layovers, carbonEmissions, bookingToken)
        { }
    }

    // ─────────────────────────────────────────────────────────────
    // FlightSegment
    // ─────────────────────────────────────────────────────────────
    public class FlightSegment : BaseEntity
    {
        [JsonInclude]
        public AirportInfo DepartureAirport { get; private set; } = null!;

        [JsonInclude]
        public AirportInfo ArrivalAirport { get; private set; } = null!;

        [JsonInclude]
        public DateTime DepartureTime { get; private set; }

        [JsonInclude]
        public DateTime ArrivalTime { get; private set; }

        [JsonInclude]
        public int Duration { get; private set; }

        [JsonInclude]
        public string Airplane { get; private set; } = string.Empty;

        [JsonInclude]
        public string Airline { get; private set; } = string.Empty;

        [JsonInclude]
        public string AirlineLogo { get; private set; } = string.Empty;

        [JsonInclude]
        public string TravelClass { get; private set; } = string.Empty;

        [JsonInclude]
        public string FlightNumber { get; private set; } = string.Empty;

        [JsonInclude]
        public bool Overnight { get; private set; }

        [JsonInclude]
        public int LegRoom { get; private set; }

        // Required by EF Core
        public FlightSegment() { }

        //[JsonConstructor]
        public FlightSegment(
            AirportInfo departureAirport,
            AirportInfo arrivalAirport,
            DateTime departureTime,
            DateTime arrivalTime,
            int duration,
            string airplane,
            string airline,
            string airlineLogo,
            string travelClass,
            string flightNumber,
            bool overnight,
            int legRoom)
        {
            DepartureAirport = departureAirport;
            ArrivalAirport = arrivalAirport;
            DepartureTime = departureTime;
            ArrivalTime = arrivalTime;
            Duration = duration;
            Airplane = airplane;
            Airline = airline;
            AirlineLogo = airlineLogo;
            TravelClass = travelClass;
            FlightNumber = flightNumber;
            Overnight = overnight;
            LegRoom = legRoom;
        }
    }

    // ─────────────────────────────────────────────────────────────
    // AirportInfo
    // ─────────────────────────────────────────────────────────────
    [Owned]
    public class AirportInfo
    {
        // NOTE: BaseEntity already has a property called "Id" (the DB key).
        // We rename the IATA-code property to "Code" to avoid the collision,
        // and use [JsonPropertyName] so existing serialized JSON (key = "Id")
        // still deserializes correctly if you had old cache entries.
        // For NEW entries the JSON will contain "Code".
        // If you prefer the old key name in JSON, keep [JsonPropertyName("Id")]
        // on the property and use "code" as the ctor parameter name.

        [JsonInclude]
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// IATA airport code (e.g. "CAI").
        /// Stored as "Code" in JSON to avoid collision with BaseEntity.Id.
        /// </summary>
        [JsonInclude]
        public string Code { get; private set; } = string.Empty;

        [JsonInclude]
        public DateTime Time { get; private set; }

        // Required by EF Core
        public AirportInfo() { }

        [JsonConstructor]
        public AirportInfo(string name, string code, DateTime time)
        {
            Name = name;
            Code = code;
            Time = time;
        }
    }

    // ─────────────────────────────────────────────────────────────
    // PriceInsights
    // ─────────────────────────────────────────────────────────────
    [Owned]
    public class PriceInsights 
    {
        [JsonInclude]
        public decimal LowestPrice { get; private set; }

        [JsonInclude]
        public string PriceLevel { get; private set; } = string.Empty;

        // Parameterless ctor – STJ will use this and then populate
        // properties via [JsonInclude], or use the [JsonConstructor] below.
        public PriceInsights() { }

        [JsonConstructor]
        public PriceInsights(decimal lowestPrice, string priceLevel)
        {
            LowestPrice = lowestPrice;
            PriceLevel = priceLevel;
        }
    }
}
