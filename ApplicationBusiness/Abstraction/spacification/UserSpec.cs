using Application.Abstraction.spacification;
using ApplicationBusiness.Fetures.TripService.Query;
using Domain.Entity.Identity;
using Domain.Entity.PostEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Abstraction.spacification
{
    internal class UserSpec : Specification<User>
    {
        public UserSpec(
            int? id,
            string? name,
            string? email,
            int? pageIndex,
            int? pageSize = 5,
            bool orderDesByTimeCreated = false)
        {
            // 1. Base Filter Logic
            if (id.HasValue)
            {
                // If searching by ID, we usually only want that specific record
                crateria = x => x.Id == id.Value;
            }
            else
            {
                Expression<Func<User, bool>> filter = u => true;

                if (!string.IsNullOrWhiteSpace(name))
                    filter = filter.AndAlso(u => u.FName.Contains(name));

                if (!string.IsNullOrWhiteSpace(email))
                    filter = filter.AndAlso(u => u.Email.Contains(email));

                crateria = filter;
            }

            // 2. Common Includes (Apply to both single and list results)
            AddStandardIncludes();

            // 3. Pagination (Only if not fetching a single ID)
            if (!id.HasValue && pageIndex.HasValue && pageIndex > 0)
            {
                int size = pageSize ?? 5;
                ApplyPagination((pageIndex.Value - 1) * size, size);
            }

            // 4. Ordering
            if (orderDesByTimeCreated)
                AddOrderByDecs(x => x.CreatedAt);
            else
                AddOrderBy(x => x.CreatedAt);
        }

        private void AddStandardIncludes()
        {
            includes.Add(x => x.TravelerCompanyProfile);
            includes.Add(x => x.TravelerProfile);
            includes.Add(x => x.TourGuideProfile);
            includes.Add(x => x.Followers);
            includes.Add(x => x.Following);

            // Use your specialized chain method for collections
            AddIncludeChain(x => x.Include(u => u.Posts)
                                    .ThenInclude(x=>x.Likes)
                                .Include(u => u.Posts)
                                    .ThenInclude(x=>x.Comments));
            //AddIncludeChain(x => x.Include(u => u.BookingPublicTrips));
            AddIncludeChain(x => x.Include(u => u.PublicTrips).ThenInclude(x=>x.PublicActivities));
        }
    }
}
