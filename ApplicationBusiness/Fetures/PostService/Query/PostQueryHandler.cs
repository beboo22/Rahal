using Application.Abstraction.message;
using Application.Abstraction.spacification;
using Application.Abstraction.Specification;
using ApplicationBusiness.Fetures.PostService.Query.Models;
using ApplicationBusiness.Fetures.PostService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.PostEntity;
using Domain.Entity.TripEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace ApplicationBusiness.Fetures.PostService.Query
{
    internal class HiringPostQueryHandler :
        IQueryHandler<GetHiringSpacificationPost, ApiResponse>
    {
        private IReadGenericRepo<HiringPost> _RPR;

        public HiringPostQueryHandler(IReadGenericRepo<HiringPost> rPR)
        {
            _RPR = rPR;
        }


        public async Task<ApiResponse> Handle(GetHiringSpacificationPost request, CancellationToken cancellationToken)
        {
            try
            {
                var spec = new HiringPostSearchSpecification(request.Date, request.Title, request.OrderDesBytimeCreated, request.page, request.capacity);

                var posts = await _RPR
                    .GetAllSpec(spec)
                    .Select(x => new HiringPostTemplate
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        UserPost = new TemplateUserPost
                        {
                            Id = x.CreatedBy.Id,
                            PrifleUser = x.CreatedBy.PhotoUrl,
                            FullName = x.CreatedBy.User.FName + " " + x.CreatedBy.User.LName,
                        },
                        PhotoUrl = x.PhotoUrl,
                        Requirements = x.Requirements,
                        Status = x.Status,
                        CreatedAt = x.CreatedAt,
                        Likes = x.Likes.Select(x => new likesSerive.Query.TemplateuserLikePost
                        {
                            LikeType = x.LikeType,
                            UserLike = new TemplateUserPost
                            {
                                Id = x.User.Id,
                                FullName = $"{x.User.FName} {x.User.LName}",

                                PrifleUser =
                                x.User.TravelerProfile != null
                                    ? x.User.TravelerProfile.PhotoUrl
                                    : x.User.TourGuideProfile != null
                                        ? x.User.TourGuideProfile.PhotoUrl
                                        : null,
                            }
                        }).ToList(),
                        numLikes = x.Likes.Count,
                        Comments = x.Comments.Select(c => new TemplateComment
                        {
                            CreatedAt = c.CreatedAt,
                            UserComment = new TemplateUserPost
                            {
                                Id = c.User.Id,
                                FullName = c.User.FName + " " + c.User.LName,
                                PrifleUser =
                                    c.User.TravelerProfile != null
                                        ? c.User.TravelerProfile.PhotoUrl
                                        : c.User.TourGuideProfile != null
                                            ? c.User.TourGuideProfile.PhotoUrl
                                            : null
                            },
                            IsEdited = c.IsEdited,
                            Msg = c.Msg,
                        }).ToList()
                    })
                    .ToListAsync();

                if (posts.Any())
                    return new ApiResultResponse<List<HiringPostTemplate>>(200, posts, "Hiring posts retrieved successfully");

                return new ApiResponse(404, "No posts found");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
    }

    internal class ExperiencePostQueryHandler :
        IQueryHandler<GetExperienceSpacificationPost, ApiResponse>
    {
        IReadGenericRepo<ExperiencePost> _RPR;

        public ExperiencePostQueryHandler(IReadGenericRepo<ExperiencePost> rPR)
        {
            _RPR = rPR;
        }


        public async Task<ApiResponse> Handle(GetExperienceSpacificationPost request, CancellationToken cancellationToken)
        {
            try
            {
                var spec = new ExperiencePostSearchSpecification(request.date, request.id, request.title, request.country,
                    request.city,
                    request.OrderDesBytimeCreated,
                    request.page, request.capacity);

                var posts = await _RPR
                    .GetAllSpec(spec)
                    .Select(p => new ExperiencePostTemplate
                    {
                        Id = p.Id,

                        UserPost = new TemplateUserPost
                        {
                            Id = p.CreatedBy.Id,
                            PrifleUser =
                            p.CreatedBy.TravelerProfile != null
                                ? p.CreatedBy.TravelerProfile.PhotoUrl
                                : p.CreatedBy.TourGuideProfile != null
                                    ? p.CreatedBy.TourGuideProfile.PhotoUrl
                                    : p.CreatedBy.TravelerCompanyProfile != null
                                        ? p.CreatedBy.TravelerCompanyProfile.PhotoUrl
                                        : null,

                            FullName = p.CreatedBy.FName + " " + p.CreatedBy.LName,
                        },
                        Likes = p.Likes.Select(x => new likesSerive.Query.TemplateuserLikePost
                        {
                            LikeType = x.LikeType,
                            UserLike = new TemplateUserPost
                            {
                                Id = x.User.Id,
                                FullName = $"{x.User.FName} {x.User.LName}",

                                PrifleUser =
                                x.User.TravelerProfile != null
                                    ? x.User.TravelerProfile.PhotoUrl
                                    : x.User.TourGuideProfile != null
                                        ? x.User.TourGuideProfile.PhotoUrl
                                        : null,
                            }
                        }).ToList(),

                        numLikes = p.Likes.Count,

                        CreatedAt = p.CreatedAt,
                        Description = p.Description,
                        PhotoUrl = p.PhotoUrl,
                        Title = p.Title,
                        City = p.City,
                        Country = p.Country,

                        Comments = p.Comments
                            .OrderByDescending(c => c.CreatedAt)
                            .Select(c => new TemplateComment
                            {
                                CreatedAt = c.CreatedAt,
                                IsEdited = c.IsEdited,
                                Msg = c.Msg,
                                UserComment = new TemplateUserPost
                                {
                                    Id = c.User.Id,
                                    FullName = c.User.FName + " " + c.User.LName,
                                    PrifleUser =
                                    c.User.TravelerProfile != null
                                        ? c.User.TravelerProfile.PhotoUrl
                                        : c.User.TourGuideProfile != null
                                            ? c.User.TourGuideProfile.PhotoUrl
                                            : null
                                }

                            }).ToList()
                    })
                    .ToListAsync();
                if (posts.Any())
                    if (request.id.HasValue)
                        return new ApiResultResponse<ExperiencePostTemplate>(200, posts.First(), "Hiring posts retrieved successfully");
                    else
                        return new ApiResultResponse<List<ExperiencePostTemplate>>(200, posts, "Hiring posts retrieved successfully");

                return new ApiResponse(404, "No posts found");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
    }
}
