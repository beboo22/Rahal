using Domain.Entity.Identity;
using Domain.Entity.TravelerEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.Hotel_flights
{
    public class HotelSearchHistory:BaseEntity
    {
        public ICollection<Hotel> Hotels { get; private set; } = new List<Hotel>();
        public string SearchId { get; private set; } = string.Empty;
        public HotelBrands Brands { get; private set; }
        public string Currency { get; private set; } = string.Empty;

        private HotelSearchHistory() { }

        public HotelSearchHistory(
            IEnumerable<Hotel> hotels,
            string searchId,
            HotelBrands brands,
            string currency)
        {
            Hotels = hotels.ToList();
            SearchId = searchId;
            Brands = brands;
            Currency = currency;
        }
    }

    public class Hotel:BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Link { get; private set; } = string.Empty;
        public decimal Rating { get; private set; }
        public int Reviews { get; private set; }
        public string Images { get; private set; }
        public decimal LowestPrice { get; private set; }
        public string PriceLabel { get; private set; } = string.Empty;
        public HotelLocation Location { get; private set; }
        public string Amenities { get; private set; }
        public string NearbyPlaces { get; private set; } 
        public string PropertyToken { get; private set; } = string.Empty;
        public bool SponsoredHotel { get; private set; }
        public int EcoLabel { get; private set; }
        public ICollection<RatePerNight> RatesPerNight { get; private set; } = new List<RatePerNight>();

        private Hotel() { }

        public Hotel(
            string name,
            string description,
            string link,
            decimal rating,
            int reviews,
            IEnumerable<HotelImage> images,
            decimal lowestPrice,
            string priceLabel,
            HotelLocation location,
            IEnumerable<string> nearbyPlaces,
            string propertyToken,
            bool sponsoredHotel,
            int ecoLabel,
            IEnumerable<RatePerNight> ratesPerNight,
            IEnumerable<string> amenities
            )
        {
            Amenities = string.Join(",", amenities);
            Name = name;
            Description = description;
            Link = link;
            Rating = rating;
            Reviews = reviews;
            Images = string.Join(",", images);
            LowestPrice = lowestPrice;
            PriceLabel = priceLabel;
            Location = location;
            NearbyPlaces = string.Join(",", nearbyPlaces);
            PropertyToken = propertyToken;
            SponsoredHotel = sponsoredHotel;
            EcoLabel = ecoLabel;
            RatesPerNight = ratesPerNight.ToList();
        }
    }

    

    public class HotelImage:BaseEntity
    {
        public string Images { get; set; }
    }

    public class HotelLocation:BaseEntity
    {
        public decimal Latitude { get; private set; }
        public decimal Longitude { get; private set; }

        private HotelLocation() { }

        public HotelLocation(decimal latitude, decimal longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    public class RatePerNight:BaseEntity
    {
        public decimal Lowest { get; private set; }
        public decimal BeforeTaxesFees { get; private set; }

        private RatePerNight() { }

        public RatePerNight(decimal lowest, decimal beforeTaxesFees)
        {
            Lowest = lowest;
            BeforeTaxesFees = beforeTaxesFees;
        }
    }

    public class HotelBrands:BaseEntity
    {
        public string Names { get; private set; }

        public HotelBrands() { }

    }

    public class PayHotel : BaseEntity
    {
        public Hotel Hotel { get; set; }
        public int HotelId { get; set; }
        public bool IsPaid { get; set; } = false;
        public bool Canceled { get; set; }
        public User User { get; set; }
        public decimal TotalBookingPrice { get; set; }
    }



}
