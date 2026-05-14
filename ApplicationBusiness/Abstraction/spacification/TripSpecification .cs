using Application.Abstraction.spacification;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.Entity.TripEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Abstraction.spacification
{
    public class PublicTripSpecification : Specification<PublicTrip>
    {
        public PublicTripSpecification(TripFilter filter)
        {

            crateria = x => true;
            // --------------------
            // Id filter (FAST PATH)
            // --------------------
            if (filter.Id.HasValue)
            {
                crateria = x => x.Id == filter.Id.Value;

                AddIncludeChain(query => query.Include(x => x.PublicActivities));
                return;
            }

            // --------------------
            // Text filters
            // --------------------
            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                AndAlso(x => x.Title.Contains(filter.Title));
            }

            if (!string.IsNullOrWhiteSpace(filter.Destination))
            {
                AndAlso(x => x.Destination.Contains(filter.Destination));
            }

            if (!string.IsNullOrWhiteSpace(filter.From))
            {
                AndAlso(x => x.From.Contains(filter.From));
            }

            // --------------------
            // Enum filter
            // --------------------
            if (filter.TripCategory.HasValue)
            {
                AndAlso(x => x.TripCategory == filter.TripCategory.Value);
            }

            // --------------------
            // Price filters
            // --------------------
            if (filter.MinPrice.HasValue)
            {
                AndAlso(x => x.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                AndAlso(x => x.Price <= filter.MaxPrice.Value);
            }

            // --------------------
            // Members filter (safe only for PublicTrip)
            // --------------------
            //if (filter.MinMembers.HasValue)
            //{
            //    AndAlso(x => x is PublicTrip &&
            //                 ((PublicTrip)x).CurrentNumberOfMember >= filter.MinMembers.Value);
            //}

            // --------------------
            // Includes (IMPORTANT)
            // --------------------
            //includes.Add(x => x.CreatedBy);

            // optional safe includes:
            //includes.Add(x => (x as PublicTrip).PublicActivities);
            AddIncludeChain(query => query.Include(x => x.PublicActivities));
            AddIncludeChain(query => query.Include(x => x.CreatedBy));
            //includes.Add(x => (x as PrivateTrip).PrivateActivities);
            // --------------------
            // Ordering & paging
            // --------------------
            if (filter.OrderDesBytimeCreated)
                AddOrderByDecs(x => x.CreatedAt);
            else
                AddOrderBy(x => x.CreatedAt);
            //AddOrderByDecs(x => x.CreatedAt);
            if (filter.PageIndex.HasValue && filter.PageIndex > 0)
            {
                int skip = (filter.PageIndex.Value - 1) * (filter.PageSize.HasValue ? filter.PageSize.Value : 1);
                ApplyPagination(skip, (filter.PageSize.HasValue ? filter.PageSize.Value : 1));
            }
        }
    }

    public class PrivateTripSpecification : Specification<PrivateTrip>
    {
        public PrivateTripSpecification(TripFilter filter)
        {

            crateria = x => true;

            // --------------------
            // Id filter (FAST PATH)
            // --------------------
            if (filter.Id.HasValue)
            {
                crateria = x => x.Id == filter.Id.Value;

                // --------------------
                // Includes (IMPORTANT)
                // --------------------
                includes.Add(x => x.CreatedBy);

                // optional safe includes:
                //includes.Add(x => (x as PublicTrip).PublicActivities);
                AddIncludeChain(query => query.Include(x => x.PrivateActivities));


                return;
            }

            // --------------------
            // Text filters
            // --------------------
            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                AndAlso(x => x.Title.Contains(filter.Title));
            }

            if (!string.IsNullOrWhiteSpace(filter.Destination))
            {
                AndAlso(x => x.Destination.Contains(filter.Destination));
            }

            if (!string.IsNullOrWhiteSpace(filter.From))
            {
                AndAlso(x => x.From.Contains(filter.From));
            }

            // --------------------
            // Enum filter
            // --------------------
            if (filter.TripCategory.HasValue)
            {
                AndAlso(x => x.TripCategory == filter.TripCategory.Value);
            }

            // --------------------
            // Price filters
            // --------------------
            if (filter.MinPrice.HasValue)
            {
                AndAlso(x => x.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                AndAlso(x => x.Price <= filter.MaxPrice.Value);
            }

            // --------------------
            // Members filter (safe only for PublicTrip)
            // --------------------
            //if (filter.MinMembers.HasValue)
            //{
            //    AndAlso(x => x is PublicTrip &&
            //                 ((PublicTrip)x).CurrentNumberOfMember >= filter.MinMembers.Value);
            //}


            // --------------------
            // Includes (IMPORTANT)
            // --------------------
            AddIncludeChain(query => query.Include(x => x.CreatedBy));

            // optional safe includes:
            //includes.Add(x => (x as PublicTrip).PublicActivities);
            AddIncludeChain(query => query.Include(x => x.PrivateActivities));
            includes.Add(x => x.PrivateActivities);
            // --------------------
            // Ordering & paging
            // --------------------
            if (filter.OrderDesBytimeCreated)
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

    public class TripFilter
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? From { get; set; }
        public string? Destination { get; set; }
        public int? Duration { get; set; }
        public TripCategory? TripCategory { get; set; }

        public bool OrderDesBytimeCreated { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? PageIndex { get; set; } = 1;
        public int? PageSize { get; set; } = 5;
    }

}
