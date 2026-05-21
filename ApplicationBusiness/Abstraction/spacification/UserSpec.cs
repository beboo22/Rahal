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
    internal class UserSearchSpec : Specification<User>
    {
        public UserSearchSpec(string? name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                crateria = u =>
                    u.FName.Contains(name) ||
                    u.LName.Contains(name);
            }
            includes.Add(x => x.TravelerCompanyProfile);
            includes.Add(x => x.TravelerProfile);
            includes.Add(x => x.TourGuideProfile);
            includes.Add(x => x.Roles);
        }
    }
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
            
           



            if (id.HasValue)
            {
                AndAlso(x => x.Id == id.Value);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    AndAlso(u => u.FName.Contains(name) || u.LName.Contains(name));
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    AndAlso(u => u.Email.Contains(email));
                }
            }


            AddStandardIncludes();

            if (!id.HasValue && pageIndex.HasValue && pageIndex > 0)
            {
                int size = pageSize ?? 5;
                ApplyPagination((pageIndex.Value - 1) * size, size);
            }

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

            AddIncludeChain(x => x.Include(x => x.Roles).ThenInclude(x => x.Role));
        }
    }

}
