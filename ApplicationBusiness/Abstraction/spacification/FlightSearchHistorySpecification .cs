using Application.Abstraction.spacification;
using Domain.Entity.Hotel_flights;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Abstraction.spacification
{
    public class FlightSearchHistorySpecification : Specification<FlightOffer>
    {
        public FlightSearchHistorySpecification(FlightHistoryFilter filter)
        {
            crateria = x => true;

            // --------------------
            // Id filter (FAST PATH)
            // --------------------
            if (filter.Id.HasValue)
            {
                crateria = x => x.Id == filter.Id.Value;

                AddCommonIncludes();

                // For a single item, we sort by the earliest segment departure
                AddOrderByDecs(x => x.Flights.Max(f => f.DepartureTime));
                return;
            }

            // --------------------
            // Filters
            // --------------------
            if (!string.IsNullOrWhiteSpace(filter.Destination))
            {
                // Simplified to search within the Flights collection of the Offer
                AndAlso(x => x.Flights.Any(f => f.ArrivalAirport.Name.Contains(filter.Destination)));
            }

            if (filter.FromDate.HasValue)
            {
                AndAlso(x => x.Flights.Any(f => f.DepartureTime >= filter.FromDate.Value));
            }

            if (filter.ToDate.HasValue)
            {
                AndAlso(x => x.Flights.Any(f => f.DepartureTime <= filter.ToDate.Value));
            }

            if (filter.MinPrice.HasValue)
                AndAlso(x => x.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                AndAlso(x => x.Price <= filter.MaxPrice.Value);

            // --------------------
            // Ordering
            // --------------------
            // Use SelectMany logic carefully for sorting collections
            AddOrderBy(x => x.Flights.Min(f => f.DepartureTime));

            // --------------------
            // Includes
            // --------------------
            AddCommonIncludes();

            // --------------------
            // Paging
            // --------------------
            if (filter.PageIndex.HasValue && filter.PageIndex > 0)
            {
                int pageSize = filter.PageSize ?? 1;
                int skip = (filter.PageIndex.Value - 1) * pageSize;
                ApplyPagination(skip, pageSize);
            }
        }

        private void AddCommonIncludes()
        {
            // Using the IncludeChain method you created to avoid .Select() errors
            // This targets: FlightOffer -> Flights -> Airports
            AddIncludeChain(query => query
                .Include(x => x.Flights)
                    .ThenInclude(f => f.ArrivalAirport));

            AddIncludeChain(query => query
                .Include(x => x.Flights)
                    .ThenInclude(f => f.DepartureAirport));

            // Include PriceInsights if it's a navigation property
            //includes.Add(x => x.Price);
        }
    }

    public class FlightHistoryFilter
    {
        public int? Id { get; set; }
        public string? Destination { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; } 
    }
}
