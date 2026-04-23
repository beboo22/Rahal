using Domain.Entity.Identity;
using Domain.Entity.TravelerEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entity.Hotel_flights
{
    // ─────────────────────────────────────────────────────────────
    // HotelSearchHistory
    // ─────────────────────────────────────────────────────────────
    public class HotelSearchHistory : BaseEntity
    {
        [JsonInclude]
        public string SearchId { get; private set; } = string.Empty;

        [JsonInclude]
        public string Currency { get; private set; } = string.Empty;

        [JsonInclude]
        public ICollection<Hotel> Hotels { get; private set; } = new List<Hotel>();

        // Required by EF Core
        private HotelSearchHistory() { }

        /// <summary>
        /// Parameter names must match property names (case-insensitive).
        /// hotels → Hotels ✔  searchId → SearchId ✔  currency → Currency ✔
        /// </summary>
        [JsonConstructor]
        public HotelSearchHistory(
            ICollection<Hotel> hotels,
            string searchId,
            string currency)
        {
            Hotels = hotels?.ToList() ?? new List<Hotel>();
            SearchId = searchId;
            Currency = currency;
        }

        // Convenience ctor for IEnumerable call sites in application layer
        public HotelSearchHistory(
            IEnumerable<Hotel> hotels,
            string searchId,
            string currency)
            : this(hotels?.ToList() ?? new List<Hotel>(), searchId, currency)
        { }
    }

    // ─────────────────────────────────────────────────────────────
    // Hotel
    // ─────────────────────────────────────────────────────────────
    public class Hotel : BaseEntity
    {
        [JsonInclude]
        public string Name { get; private set; } = string.Empty;

        [JsonInclude]
        public string Description { get; private set; } = string.Empty;

        [JsonInclude]
        public string Link { get; private set; } = string.Empty;

        [JsonInclude]
        public decimal Rating { get; private set; }

        [JsonInclude]
        public int Reviews { get; private set; }

        /// <summary>
        /// Stored as a comma-separated string in the DB/cache.
        /// The constructor parameter type MUST match this property type (string).
        /// We expose a helper <see cref="GetImageList"/> for application use.
        /// </summary>
        [JsonInclude]
        public string Images { get; private set; } = string.Empty;

        [JsonInclude]
        public decimal LowestPrice { get; private set; }

        [JsonInclude]
        public string PriceLabel { get; private set; } = string.Empty;

        [JsonInclude]
        public HotelLocation Location { get; private set; } = null!;

        /// <summary>Comma-separated amenity names.</summary>
        [JsonInclude]
        public string Amenities { get; private set; } = string.Empty;

        /// <summary>Comma-separated nearby place names.</summary>
        [JsonInclude]
        public string NearbyPlaces { get; private set; } = string.Empty;

        [JsonInclude]
        public string PropertyToken { get; private set; } = string.Empty;

        [JsonInclude]
        public bool SponsoredHotel { get; private set; }

        [JsonInclude]
        public int EcoLabel { get; private set; }

        [JsonInclude]
        public ICollection<RatePerNight> RatesPerNight { get; private set; } = new List<RatePerNight>();

        // Required by EF Core
        private Hotel() { }

        /// <summary>
        /// [JsonConstructor] – every parameter name matches its property name exactly.
        /// 
        ///   name           → Name           ✔
        ///   description    → Description    ✔
        ///   link           → Link           ✔
        ///   rating         → Rating         ✔
        ///   reviews        → Reviews        ✔
        ///   images         → Images         ✔  (string, not IEnumerable!)
        ///   lowestPrice    → LowestPrice    ✔
        ///   priceLabel     → PriceLabel     ✔
        ///   location       → Location       ✔
        ///   amenities      → Amenities      ✔  (string, not IEnumerable!)
        ///   nearbyPlaces   → NearbyPlaces   ✔  (string, not IEnumerable!)
        ///   propertyToken  → PropertyToken  ✔
        ///   sponsoredHotel → SponsoredHotel ✔
        ///   ecoLabel       → EcoLabel       ✔
        ///   ratesPerNight  → RatesPerNight  ✔
        /// </summary>
        [JsonConstructor]
        public Hotel(
            string name,
            string description,
            string link,
            decimal rating,
            int reviews,
            string images,             // ← matches property type string
            decimal lowestPrice,
            string priceLabel,
            HotelLocation location,
            string amenities,          // ← matches property type string
            string nearbyPlaces,       // ← matches property type string
            string propertyToken,
            bool sponsoredHotel,
            int ecoLabel,
            ICollection<RatePerNight> ratesPerNight)
        {
            Name = name;
            Description = description;
            Link = link;
            Rating = rating;
            Reviews = reviews;
            Images = images ?? string.Empty;
            LowestPrice = lowestPrice;
            PriceLabel = priceLabel;
            Location = location;
            Amenities = amenities ?? string.Empty;
            NearbyPlaces = nearbyPlaces ?? string.Empty;
            PropertyToken = propertyToken;
            SponsoredHotel = sponsoredHotel;
            EcoLabel = ecoLabel;
            RatesPerNight = ratesPerNight?.ToList() ?? new List<RatePerNight>();
        }

        // ── Convenience factory used by the application/mapping layer ──
        // Accepts the IEnumerable<string> lists that come from the API DTO
        // and converts them to comma-separated strings before calling the
        // canonical [JsonConstructor] above.
        public static Hotel Create(
            string name,
            string description,
            string link,
            decimal rating,
            int reviews,
            IEnumerable<string>? images,
            decimal lowestPrice,
            string priceLabel,
            HotelLocation location,
            IEnumerable<string>? nearbyPlaces,
            string propertyToken,
            bool sponsoredHotel,
            int ecoLabel,
            IEnumerable<RatePerNight>? ratesPerNight,
            IEnumerable<string>? amenities)
        {
            return new Hotel(
                name,
                description,
                link,
                rating,
                reviews,
                images != null ? string.Join(",", images) : string.Empty,
                lowestPrice,
                priceLabel,
                location,
                amenities != null ? string.Join(",", amenities) : string.Empty,
                nearbyPlaces != null ? string.Join(",", nearbyPlaces) : string.Empty,
                propertyToken,
                sponsoredHotel,
                ecoLabel,
                ratesPerNight?.ToList() ?? new List<RatePerNight>()
            );
        }

        // ── Helpers for reading back the comma-separated strings ──────
        public IReadOnlyList<string> GetImageList() =>
            string.IsNullOrEmpty(Images)
                ? Array.Empty<string>()
                : Images.Split(',', StringSplitOptions.RemoveEmptyEntries);

        public IReadOnlyList<string> GetAmenitiesList() =>
            string.IsNullOrEmpty(Amenities)
                ? Array.Empty<string>()
                : Amenities.Split(',', StringSplitOptions.RemoveEmptyEntries);

        public IReadOnlyList<string> GetNearbyPlacesList() =>
            string.IsNullOrEmpty(NearbyPlaces)
                ? Array.Empty<string>()
                : NearbyPlaces.Split(',', StringSplitOptions.RemoveEmptyEntries);
    }

    // ─────────────────────────────────────────────────────────────
    // HotelImage  (unchanged)
    // ─────────────────────────────────────────────────────────────
    public class HotelImage : BaseEntity
    {
        public string Images { get; set; } = string.Empty;
    }

    // ─────────────────────────────────────────────────────────────
    // HotelLocation
    // ─────────────────────────────────────────────────────────────
    public class HotelLocation : BaseEntity
    {
        [JsonInclude]
        public decimal Latitude { get; private set; }

        [JsonInclude]
        public decimal Longitude { get; private set; }

        // Required by EF Core
        private HotelLocation() { }

        [JsonConstructor]
        public HotelLocation(decimal latitude, decimal longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    // ─────────────────────────────────────────────────────────────
    // RatePerNight
    // ─────────────────────────────────────────────────────────────
    public class RatePerNight : BaseEntity
    {
        [JsonInclude]
        public decimal Lowest { get; private set; }

        [JsonInclude]
        public decimal BeforeTaxesFees { get; private set; }

        // Required by EF Core
        private RatePerNight() { }

        [JsonConstructor]
        public RatePerNight(decimal lowest, decimal beforeTaxesFees)
        {
            Lowest = lowest;
            BeforeTaxesFees = beforeTaxesFees;
        }
    }

    // ─────────────────────────────────────────────────────────────
    // HotelBrands  (unchanged)
    // ─────────────────────────────────────────────────────────────
    public class HotelBrands : BaseEntity
    {
        public string Names { get; private set; } = string.Empty;
        public HotelBrands() { }
    }

    // ─────────────────────────────────────────────────────────────
    // PayHotel  (unchanged)
    // ─────────────────────────────────────────────────────────────
    public class PayHotel : BaseEntity
    {
        public Hotel Hotel { get; set; } = null!;
        public int HotelId { get; set; }
        public bool IsPaid { get; set; } = false;
        public bool Canceled { get; set; }
        public User User { get; set; } = null!;
        public decimal TotalBookingPrice { get; set; }
    }
}
