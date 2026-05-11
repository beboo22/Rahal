using Application.Abstraction.spacification;
using ApplicationBusiness.Fetures.TripService.Query;
using Domain.Entity.Identity;
using Domain.Entity.PostEntity;
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
            bool OrderDesBytimeCreated=false
            )
        {
            Expression<Func<User, bool>> _criteria = u => true;



            if (id.HasValue)
            {
                crateria = x => x.Id == id.Value;

                // --------------------
                // Includes (IMPORTANT)
                // --------------------
                includes.Add(x => x.TravelerCompanyProfile);
                includes.Add(x => x.TravelerProfile);
                includes.Add(x => x.TourGuideProfile);


                return;
            }

            if (!string.IsNullOrWhiteSpace(name))
                _criteria = _criteria.AndAlso(u => u.FName.Contains(name));


            if (!string.IsNullOrWhiteSpace(email))
                _criteria = _criteria.AndAlso(u => u.Email.Contains(email));



            crateria = _criteria;

            includes.Add(x => x.TravelerCompanyProfile);
            includes.Add(x => x.TravelerProfile);
            includes.Add(x => x.TourGuideProfile);
            // Pagination
            if (pageIndex.HasValue && pageIndex > 0)
            {
                int skip = (pageIndex.Value - 1) * (pageSize.HasValue ? pageSize.Value : 1);
                ApplyPagination(skip, (pageSize.HasValue ? pageSize.Value : 1));
            }

            if (OrderDesBytimeCreated)
                AddOrderByDecs(x => x.CreatedAt);
            else
                AddOrderBy(x => x.CreatedAt);
        }

    }
}
