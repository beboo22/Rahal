using Domain.Entity;
using Domain.Entity.Hotel_flights;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Hotels
{
    public class HotelSearchRequest
    {
        [Required]
        public string Destination { get; set; } = string.Empty;

        [Required]
        public string CheckInDate { get; set; } = string.Empty;

        [Required]
        public string CheckOutDate { get; set; } = string.Empty;

        [Range(1, 30)]
        public int Adults { get; set; } = 2;

        [Range(0, 10)]
        public int Children { get; set; } = 0;
        public List<int> ChildrenAges { get;  set; }

        [Range(1, 10)]
        public int Rooms { get; set; } = 1;

        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        /// <summary>Minimum hotel star rating (1–5)</summary>
        //[Range(1, 5)]
        //public int? MinRating { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        /// <summary>Amenity filter IDs (e.g. 35=pool, 9=free breakfast)</summary>
        public List<int> Amenities { get; set; } = new();

        //public HotelSortOption SortBy { get; set; } = HotelSortOption.Relevance;

        [StringLength(5)]
        public string Gl { get; set; } = "us";

        [StringLength(5)]
        public string Hl { get; set; } = "en";

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class HotelSearchResponse
    {
        public List<HotelResult> Properties { get; set; } = new();
        public string SearchId { get; set; } = string.Empty;
        public BrandFilters? Brands { get; set; }
        public string currency { get; set; }
    }

    public class HotelResult
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int Reviews { get; set; }
        public List<string> Images { get; set; } = new();
        public decimal LowestPrice { get; set; }
        public string PriceLabel { get; set; } = string.Empty;
        public HotelLocation Location { get; set; } = new();
        public List<string> Amenities { get; set; } = new();
        public List<string> NearbyPlaces { get; set; } = new();
        public string PropertyToken { get; set; } = string.Empty;
        public bool SponsoredHotel { get; set; }
        public int? EcoLabel { get; set; }
        public List<PriceSummary> RatePerNight { get; set; } = new();
    }

    public class HotelLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class PriceSummary
    {
        public decimal Lowest { get; set; }
        public decimal BeforeTaxesFees { get; set; }
    }

    public class BrandFilters
    {
        public List<string> Names { get; set; } = new();
    }
}
