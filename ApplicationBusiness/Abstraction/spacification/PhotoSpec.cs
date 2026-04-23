using Application.Abstraction.spacification;
using Domain.Entity.Hotel_flights;
using Domain.Entity.photo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Abstraction.spacification
{


    public class PhotoSpec : Specification<PhotoSearchResponse>
    {
        public PhotoSpec(PhotoFilter filter)
        {
            crateria = x => true;

            if (!string.IsNullOrWhiteSpace(filter.SearchId))
            {
                var search = filter.SearchId.ToLower();
                AndAlso(x =>
                    x.SearchId.ToLower().Contains(search)
                    );
            }

            includes.Add(x =>x.Images);

            AddOrderByDecs(x => x.CreatedAt);

            ApplyPagination(filter.PageIndex, filter.PageSize);
        }
    }

    public class PhotoFilter
    {
        public string? SearchId { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }


}
