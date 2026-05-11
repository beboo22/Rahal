using Application.Abstraction.spacification;
using Domain.Entity.Hotel_flights;
using Domain.Entity.photo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Abstraction.spacification
{


    public class PhotoSpec : Specification<PhotoSearchResponse>
    {
        public static string NormalizeSearch(string input)
        {
            return input.Trim()
                        .ToLower()
                        .Replace(" ", "");
        }
        public PhotoSpec(PhotoFilter filter)
        {
            crateria = x => true;





            if (!string.IsNullOrWhiteSpace(filter.SearchId))
            {
                var normalized = NormalizeSearch(filter.SearchId);

                crateria = x =>
                    EF.Functions.Like(
                        x.SearchId.ToLower(),
                        $"%{normalized}%");


                var search = filter.SearchId.ToLower();
                AndAlso(x =>
                    x.SearchId.ToLower().Contains(search)
                    );
            }

            includes.Add(x =>x.Images);

            AddOrderByDecs(x => x.CreatedAt);

            //ApplyPagination(filter.PageIndex, filter.PageSize);
            if (filter.PageIndex.HasValue && filter.PageIndex > 0)
            {
                int skip = (filter.PageIndex.Value - 1) * (filter.PageSize.HasValue ? filter.PageSize.Value : 1);
                ApplyPagination(skip, (filter.PageSize.HasValue ? filter.PageSize.Value : 1));
            }
        }
    }

    public class PhotoFilter
    {
        public string? SearchId { get; set; }

        public int? PageIndex { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }


}
