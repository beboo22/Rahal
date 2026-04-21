using Application.Abstraction.spacification;
using Domain.Entity.Hotel_flights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Abstraction.spacification
{

    public class HotelSearchHistorySpecification : Specification<HotelSearchHistory>
    {
        public HotelSearchHistorySpecification(HotelHistoryFilter filter)
        {
            crateria = x => true;

            if (!string.IsNullOrWhiteSpace(filter.Destination))
            {
                AndAlso(x =>
                    x.Hotels.Any(h =>
                        h.Name.Contains(filter.Destination) ||
                        h.NearbyPlaces.Contains(filter.Destination)));
            }

            if (filter.MinPrice.HasValue)
            {
                AndAlso(x =>
                    x.Hotels.Any(h =>
                        h.LowestPrice >= filter.MinPrice.Value));
            }

            if (filter.MaxPrice.HasValue)
            {
                AndAlso(x =>
                    x.Hotels.Any(h =>
                        h.LowestPrice <= filter.MaxPrice.Value));
            }

            if (filter.MinRating.HasValue)
            {
                AndAlso(x =>
                    x.Hotels.Any(h =>
                        h.Rating >= filter.MinRating.Value));
            }

            AddOrderByDecs(x => x.CreatedAt);

            ApplyPagination(filter.PageIndex, filter.PageSize);
        }
    }

    public class HotelHistoryFilter
    {
        public string? UserId { get; set; }
        public string? Destination { get; set; }
        public DateTime? CheckInFrom { get; set; }
        public DateTime? CheckInTo { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinRating { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
