using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PostService.Query.Models
{
    public record GetHiringSpacificationPost(DateTime? Date,int?id, string? Title, int? page, bool OrderDesBytimeCreated=false,int capacity=5) : IQuery<ApiResponse>;



   
    public record GetExperienceSpacificationPost(DateTime? date,
        int? id,
            string? title,
            string? country,
            string? city,
            bool OrderDesBytimeCreated,
            decimal? budget,int? page,int capacity=5) : IQuery<ApiResponse>;
}
