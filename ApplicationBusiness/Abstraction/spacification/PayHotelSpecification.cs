using Application.Abstraction.spacification;
using Domain.Entity.Hotel_flights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Abstraction.spacification
{
    public class PaymentFilter
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public bool? IsPaid { get; set; }
        public bool? Canceled { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Paging & Sorting
        public bool OrderDescending { get; set; } = true;
        public int? PageIndex { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
    public class PayHotelSpecification : Specification<PayHotel>
    {
        public PayHotelSpecification(PaymentFilter filter)
        {
            crateria = x => true;

            // Id Fast Path
            if (filter.Id.HasValue)
            {
                crateria = x => x.Id == filter.Id.Value;
                //includes.Add(x => x.Hotel);
                //includes.Add(x => x.User);
                return;
            }

            // Filters
            if (filter.UserId.HasValue)
                AndAlso(x => x.UserId == filter.UserId.Value);

            if (filter.IsPaid.HasValue)
                AndAlso(x => x.IsPaid == filter.IsPaid.Value);

            if (filter.Canceled.HasValue)
                AndAlso(x => x.Canceled == filter.Canceled.Value);

            if (filter.MinPrice.HasValue)
                AndAlso(x => x.TotalBookingPrice >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                AndAlso(x => x.TotalBookingPrice <= filter.MaxPrice.Value);

            // Includes
            //includes.Add(x => x.Hotel);
            //includes.Add(x => x.User);

            // Ordering & Paging
            if (filter.OrderDescending)
                AddOrderByDecs(x => x.CreatedAt);
            else
                AddOrderBy(x => x.CreatedAt);

            if (filter.PageIndex.HasValue && filter.PageIndex > 0)
            {
                int skip = (filter.PageIndex.Value - 1) * (filter.PageSize.HasValue ? filter.PageSize.Value : 1);
                ApplyPagination(skip, (filter.PageSize.HasValue ? filter.PageSize.Value : 1));
            }
        }
    }

    public class PayFlightSpecification : Specification<PayFlight>
    {
        public PayFlightSpecification(PaymentFilter filter)
        {
            crateria = x => true;

            if (filter.Id.HasValue)
            {
                crateria = x => x.FlightOffer.Id == filter.Id.Value;
                includes.Add(x => x.FlightOffer);
                includes.Add(x => x.User);
                return;
            }

            // Filters
            if (filter.UserId.HasValue)
                AndAlso(x => x.User.Id == filter.UserId.Value); // Adjust if PayFlight has UserId prop

            if (filter.IsPaid.HasValue)
                AndAlso(x => x.IsPaid == filter.IsPaid.Value);

            if (filter.Canceled.HasValue)
                AndAlso(x => x.Canceled == filter.Canceled.Value);

            if (filter.MinPrice.HasValue)
                AndAlso(x => x.TotalBookingPrice >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                AndAlso(x => x.TotalBookingPrice <= filter.MaxPrice.Value);

            // Includes
            //includes.Add(x => x.FlightOffer);
            //includes.Add(x => x.User);

            // Ordering & Paging
            if (filter.OrderDescending)
                AddOrderByDecs(x => x.CreatedAt);
            else
                AddOrderBy(x => x.CreatedAt);

            if (filter.PageIndex.HasValue && filter.PageIndex > 0)
            {
                int skip = (filter.PageIndex.Value - 1) * (filter.PageSize.HasValue ? filter.PageSize.Value : 1);
                ApplyPagination(skip, (filter.PageSize.HasValue ? filter.PageSize.Value : 1));
            }
        }
    }

}
