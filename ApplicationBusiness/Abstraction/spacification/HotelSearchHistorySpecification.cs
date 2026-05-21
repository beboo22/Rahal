using Application.Abstraction.spacification;
using Domain.Entity.Hotel_flights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Abstraction.spacification
{

    //public class HotelSearchHistorySpecification : Specification<HotelSearchHistory>
    //{
    //    public HotelSearchHistorySpecification(HotelHistoryFilter filter)
    //    {
    //        includes.Add(x => x.Hotels);
    //        // Start with a broad criteria
    //        crateria = x => x.Hotels.Any(h =>
    //            (!filter.Id.HasValue || h.Id == filter.Id.Value) &&
    //            (string.IsNullOrWhiteSpace(filter.Destination) || h.Name.Contains(filter.Destination) || h.NearbyPlaces.Contains(filter.Destination)) &&
    //            (!filter.MinPrice.HasValue || h.LowestPrice >= filter.MinPrice.Value) &&
    //            (!filter.MaxPrice.HasValue || h.LowestPrice <= filter.MaxPrice.Value) &&
    //            (!filter.MinRating.HasValue || h.Rating >= filter.MinRating.Value)
    //        );

    //        AddOrderByDecs(x => x.CreatedAt);
    //        ApplyPagination(filter.PageIndex, filter.PageSize);
    //    }

    //public HotelSearchHistorySpecification(HotelHistoryFilter filter)
    //{
    //    crateria = x => true;
    //    if(filter.Id.HasValue)
    //        AndAlso(x =>
    //            x.Hotels.Any(h => h.Id == filter.Id.Value));


    //    if (!string.IsNullOrWhiteSpace(filter.Destination))
    //    {
    //        AndAlso(x =>
    //            x.Hotels.Any(h =>
    //                h.Name.Contains(filter.Destination) ||
    //                h.NearbyPlaces.Contains(filter.Destination)));
    //    }

    //    if (filter.MinPrice.HasValue)
    //    {
    //        AndAlso(x =>
    //            x.Hotels.Any(h =>
    //                h.LowestPrice >= filter.MinPrice.Value));
    //    }

    //    if (filter.MaxPrice.HasValue)
    //    {
    //        AndAlso(x =>
    //            x.Hotels.Any(h =>
    //                h.LowestPrice <= filter.MaxPrice.Value));
    //    }

    //    if (filter.MinRating.HasValue)
    //    {
    //        AndAlso(x =>
    //            x.Hotels.Any(h =>
    //                h.Rating >= filter.MinRating.Value));
    //    }

    //    AddOrderByDecs(x => x.CreatedAt);

    //    ApplyPagination(filter.PageIndex, filter.PageSize);
    //}

    //}

    public class HotelSpecification : Specification<Hotel>
    {
        public HotelSpecification(HotelHistoryFilter filter)
        {

            crateria = x => true;
            if (filter.Id.HasValue)
            {
                crateria = x => x.Id == filter.Id.Value;
                // --------------------
                // Includes (IMPORTANT)
                // --------------------
                includes.Add(x => x.Location);

                // --------------------
                // Sorting
                // --------------------
                AddOrderByDecs(x => x.CreatedAt);
                return;
            }

            

            // --------------------
            // Id filter
            // --------------------

            // --------------------
            // Text search (Name + NearbyPlaces)
            // --------------------
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                AndAlso(x =>
                    x.Name.Contains(filter.Name));
            }

            if (!string.IsNullOrWhiteSpace(filter.Destination))
            {
                AndAlso(x =>
                    x.Name.Contains(filter.Destination) ||
                    x.NearbyPlaces.Contains(filter.Destination));
            }

            // --------------------
            // Price filters
            // --------------------
            if (filter.MinPrice.HasValue)
            {
                AndAlso(x => x.LowestPrice >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                AndAlso(x => x.LowestPrice <= filter.MaxPrice.Value);
            }

            // --------------------
            // Rating filter
            // --------------------
            if (filter.MinRating.HasValue)
            {
                AndAlso(x => x.Rating >= filter.MinRating.Value);
            }
            // --------------------
            // Includes (IMPORTANT)
            // --------------------
            includes.Add(x => x.Location);

            // --------------------
            // Sorting
            // --------------------
            AddOrderByDecs(x => x.CreatedAt);

            // --------------------
            // Pagination
            // --------------------
            //ApplyPagination(filter.PageIndex, filter.PageSize);
            if (filter.PageIndex.HasValue && filter.PageIndex > 0)
            {
                int skip = (filter.PageIndex.Value - 1) * (filter.PageSize.HasValue ? filter.PageSize.Value : 1);
                ApplyPagination(skip, (filter.PageSize.HasValue ? filter.PageSize.Value : 1));
            }
        }
    }

    public class HotelSearchHistorySpecification : Specification<HotelSearchHistory>
    {
        public HotelSearchHistorySpecification(HotelHistoryFilter filter)
        {
            includes.Add(x => x.Hotels);

            // -------------------------
            // Parent-level filters
            // -------------------------
            if (filter.Id.HasValue)
            {
                crateria = x => x.Hotels.Any(h => h.Id == filter.Id.Value);
            }
            else
            {
                crateria = x => true;
            }

            // -------------------------
            // Child-level filters
            // -------------------------

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

            // -------------------------
            // Sorting & paging
            // -------------------------
            AddOrderByDecs(x => x.CreatedAt);
            //ApplyPagination(filter.PageIndex, filter.PageSize);

            if (filter.PageIndex.HasValue && filter.PageIndex > 0)
            {
                int skip = (filter.PageIndex.Value - 1) * (filter.PageSize.HasValue ? filter.PageSize.Value : 1);
                ApplyPagination(skip, (filter.PageSize.HasValue ? filter.PageSize.Value : 1));
            }

        }
    }
    public class HotelHistoryFilter
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Destination { get; set; }
        public DateTime? CheckInFrom { get; set; }
        public DateTime? CheckInTo { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinRating { get; set; }
        public int? PageIndex { get; set; } 
        public int? PageSize { get; set; } 
    }

}
