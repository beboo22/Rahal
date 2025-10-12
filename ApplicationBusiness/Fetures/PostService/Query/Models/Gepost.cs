using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PostService.Query.Models
{
    public record GetRecentHiringPost : IQuery<ApiResponse>;
    public record GetHiringPost(DateTime Date) : IQuery<ApiResponse>;
    public record GetLastWeekHiringPost : IQuery<ApiResponse>;
    public record GetLastMonthHiringPost : IQuery<ApiResponse>;
    public record GetLastYearHiringPost : IQuery<ApiResponse>;
    public record GetHiringPostByTitle(string Title): IQuery<ApiResponse>;


    public record GetExperiencePost(DateTime Date) : IQuery<ApiResponse>;
    public record GetLastMonthExperiencePost : IQuery<ApiResponse>;
    public record GetLastYearExperiencePost : IQuery<ApiResponse>;
    public record GetRecentExperiencePost : IQuery<ApiResponse>;
    public record GetLastWeekExperiencePost : IQuery<ApiResponse>;
    public record GetExperiencePostByTitle(string Title): IQuery<ApiResponse>;
}
