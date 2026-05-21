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
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; } 
    }
    public class PayHotelSpecification : Specification<PayHotel>
    {
        public PayHotelSpecification(PaymentFilter filter)
        {
            // تم إزالة crateria = x => true لمنع توليد شروط SQL زائدة (1=1)

            // Id Fast Path
            if (filter.Id.HasValue)
            {
                AndAlso(x => x.Id == filter.Id.Value);
                IncludeHotelAndUser(); // يمكنك تفعيلها هنا أيضاً إذا أردت عند البحث بالـ ID
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

            // Ordering & Paging
            if (filter.OrderDescending)
                AddOrderByDecs(x => x.CreatedAt);
            else
                AddOrderBy(x => x.CreatedAt);

            if (filter.PageIndex.HasValue && filter.PageIndex > 0)
            {
                int size = filter.PageSize ?? 5; // قيمة افتراضية مناسبة بدلاً من 1 لمنع مشاكل الـ Paging
                int skip = (filter.PageIndex.Value - 1) * size;
                ApplyPagination(skip, size);
            }
        }

        // الدالة المطلوبة لعمل الـ Includes الخاصة بالفندق والمستخدم
        public PayHotelSpecification IncludeHotelAndUser()
        {
            includes.Add(x => x.Hotel);
            includes.Add(x => x.User);
            return this; // تتيح لك استخدام الـ Chaining عند بناء الـ Object
        }
    }
    public class PayFlightSpecification : Specification<PayFlight>
    {
        public PayFlightSpecification(PaymentFilter filter)
        {
            // تم إزالة crateria = x => true لمنع توليد شروط SQL زائدة (1=1)

            if (filter.Id.HasValue)
            {
                AndAlso(x => x.FlightOffer.Id == filter.Id.Value);
                IncludeFlightOfferAndUser();
                return;
            }

            // Filters
            if (filter.UserId.HasValue)
                AndAlso(x => x.User.Id == filter.UserId.Value);

            if (filter.IsPaid.HasValue)
                AndAlso(x => x.IsPaid == filter.IsPaid.Value);

            if (filter.Canceled.HasValue)
                AndAlso(x => x.Canceled == filter.Canceled.Value);

            if (filter.MinPrice.HasValue)
                AndAlso(x => x.TotalBookingPrice >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                AndAlso(x => x.TotalBookingPrice <= filter.MaxPrice.Value);

            // Ordering & Paging
            if (filter.OrderDescending)
                AddOrderByDecs(x => x.CreatedAt);
            else
                AddOrderBy(x => x.CreatedAt);

            if (filter.PageIndex.HasValue && filter.PageIndex > 0)
            {
                int size = filter.PageSize ?? 5;
                int skip = (filter.PageIndex.Value - 1) * size;
                ApplyPagination(skip, size);
            }
        }

        // الدالة المطلوبة لعمل الـ Includes الخاصة برحلة الطيران والمستخدم
        public PayFlightSpecification IncludeFlightOfferAndUser()
        {
            AddIncludeChain(x => x.Include(x=>x.FlightOffer).ThenInclude(x=>x.Flights));
            //AddIncludeChain(x => x.Include(x=>x.FlightOffer).ThenInclude(x=>x.Flights).ThenInclude(x=>x.DepartureAirport));
            includes.Add(x => x.User);
            return this; // تتيح لك استخدام الـ Chaining عند بناء الـ Object
        }
    }
}
