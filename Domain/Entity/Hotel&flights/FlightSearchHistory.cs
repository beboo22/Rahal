using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.Hotel_flights
{
    public class FlightSearchHistory:BaseEntity
    {
        public IReadOnlyCollection<FlightOffer> BestFlights { get; private set; } = new List<FlightOffer>();
        public IReadOnlyCollection<FlightOffer> OtherFlights { get; private set; } = new List<FlightOffer>();
        public PriceInsights PriceInsights { get; private set; }
        public string SearchId { get; private set; } = string.Empty;
        public string Currency { get; private set; } = string.Empty;
        private FlightSearchHistory() { }

        public FlightSearchHistory(
            IEnumerable<FlightOffer> bestFlights,
            IEnumerable<FlightOffer> otherFlights,
            PriceInsights priceInsights,
            string searchId,
            string currency)
        {
            BestFlights = bestFlights.ToList();
            OtherFlights = otherFlights.ToList();
            PriceInsights = priceInsights;
            SearchId = searchId;
            Currency = currency;
        }

    }

    public class FlightOffer:BaseEntity
    {
        public IReadOnlyCollection<FlightSegment> Flights { get; private set; } = new List<FlightSegment>();
        public int TotalDuration { get; private set; }
        public decimal Price { get; private set; }
        public string Type { get; private set; } = string.Empty;
        public IReadOnlyCollection<string> AirlineLogos { get; private set; } = new List<string>();
        public int Layovers { get; private set; }
        public bool CarbonEmissions { get; private set; }
        public string BookingToken { get; private set; } = string.Empty;

        private FlightOffer() { }

        public FlightOffer(
            IEnumerable<FlightSegment> flights,
            int totalDuration,
            decimal price,
            string type,
            IEnumerable<string> airlineLogos,
            int layovers,
            bool carbonEmissions,
            string bookingToken)
        {
            Flights = flights.ToList();
            TotalDuration = totalDuration;
            Price = price;
            Type = type;
            AirlineLogos = airlineLogos.ToList();
            Layovers = layovers;
            CarbonEmissions = carbonEmissions;
            BookingToken = bookingToken;
        }
    }

    public class FlightSegment:BaseEntity
    {
        public AirportInfo DepartureAirport { get; private set; }
        public AirportInfo ArrivalAirport { get; private set; }
        public DateTime DepartureTime { get; private set; }
        public DateTime ArrivalTime { get; private set; }
        public int Duration { get; private set; }
        public string Airplane { get; private set; } = string.Empty;
        public string Airline { get; private set; } = string.Empty;
        public string AirlineLogo { get; private set; } = string.Empty;
        public string TravelClass { get; private set; } = string.Empty;
        public string FlightNumber { get; private set; } = string.Empty;
        public bool Overnight { get; private set; }
        public int LegRoom { get; private set; }

        private FlightSegment() { }

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

    public class AirportInfo:BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Id { get; private set; } = string.Empty;
        public DateTime Time { get; private set; }

        private AirportInfo() { }

        public AirportInfo(string name, string id, DateTime time)
        {
            Name = name;
            Id = id;
            Time = time;
        }
    }

    public class PriceInsights:BaseEntity
    {
        public decimal LowestPrice { get; private set; }
        public string PriceLevel { get; private set; } = string.Empty;
        public ICollection<List<decimal>> PriceHistory { get; private set; }
            = new List<List<decimal>>();

        private PriceInsights() { }

        public PriceInsights(
            decimal lowestPrice,
            string priceLevel,
            IEnumerable<IEnumerable<decimal>> priceHistory)
        {
            LowestPrice = lowestPrice;
            PriceLevel = priceLevel;
            PriceHistory = priceHistory
                .Select(x => (List<decimal>)x.ToList())
                .ToList();
        }
    }









    
}
