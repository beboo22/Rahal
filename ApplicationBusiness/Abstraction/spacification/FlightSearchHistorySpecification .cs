using Application.Abstraction.spacification;
using Domain.Entity.Hotel_flights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Abstraction.spacification
{
    public class FlightSearchHistorySpecification : Specification<FlightSearchHistory>
    {
        public FlightSearchHistorySpecification(FlightHistoryFilter filter)
        {
            crateria = x => true;

            if (!string.IsNullOrWhiteSpace(filter.Destination))
            {
                AndAlso(x =>
                    x.BestFlights.Any(f =>
                        f.Flights.Any(s =>
                            s.ArrivalAirport.Name.Contains(filter.Destination)))
                    ||
                    x.OtherFlights.Any(f =>
                        f.Flights.Any(s =>
                            s.ArrivalAirport.Name.Contains(filter.Destination))));
            }

            if (filter.FromDate.HasValue)
            {
                AndAlso(x =>
                    x.BestFlights.Any(f =>
                        f.Flights.Any(s =>
                            s.DepartureTime >= filter.FromDate.Value))
                    ||
                    x.OtherFlights.Any(f =>
                        f.Flights.Any(s =>
                            s.DepartureTime >= filter.FromDate.Value)));
            }

            if (filter.ToDate.HasValue)
            {
                AndAlso(x =>
                    x.BestFlights.Any(f =>
                        f.Flights.Any(s =>
                            s.DepartureTime <= filter.ToDate.Value))
                    ||
                    x.OtherFlights.Any(f =>
                        f.Flights.Any(s =>
                            s.DepartureTime <= filter.ToDate.Value)));
            }

            if (filter.MinPrice.HasValue)
                AndAlso(x => x.PriceInsights.LowestPrice >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                AndAlso(x => x.PriceInsights.LowestPrice <= filter.MaxPrice.Value);

            AddOrderByDecs(x => x.CreatedAt);

            ApplyPagination(filter.PageIndex, filter.PageSize);
        }
    }

    public class FlightHistoryFilter
    {
        public string? UserId { get; set; }
        public string? Destination { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
