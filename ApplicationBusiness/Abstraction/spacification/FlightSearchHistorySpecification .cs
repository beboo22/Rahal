using Application.Abstraction.spacification;
using Domain.Entity.Hotel_flights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Abstraction.spacification
{
    //public class FlightSearchHistorySpecification : Specification<FlightSearchHistory>
    //{
    //    public FlightSearchHistorySpecification(FlightHistoryFilter filter)
    //    {
    //        crateria = x => true;

    //        if (!string.IsNullOrWhiteSpace(filter.UserId))
    //            AndAlso(x => x.UserId == filter.UserId);

    //        if (!string.IsNullOrWhiteSpace(filter.Destination))
    //            AndAlso(x => x.ArrivalId.Contains(filter.Destination));

    //        if (filter.FromDate.HasValue)
    //            AndAlso(x => x.DepartureDate >= filter.FromDate.Value);

    //        if (filter.ToDate.HasValue)
    //            AndAlso(x => x.DepartureDate <= filter.ToDate.Value);

    //        if (filter.MinPrice.HasValue)
    //            AndAlso(x => x.MinPrice >= filter.MinPrice.Value);

    //        if (filter.MaxPrice.HasValue)
    //            AndAlso(x => x.MaxPrice <= filter.MaxPrice.Value);

    //        AddOrderByDecs(x => x.CreatedAt);
    //        ApplyPagination(filter.PageIndex, filter.PageSize);
    //    }
    //}

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
