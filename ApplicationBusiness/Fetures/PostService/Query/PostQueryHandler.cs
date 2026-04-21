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

namespace ApplicationBusiness.Fetures.PostService.Query
{
    internal class HiringPostQueryHandler : IQueryHandler<GetHiringPostByTitle, ApiResponse>,
        IQueryHandler<GetHiringPost, ApiResponse>,
        IQueryHandler<GetHiringSpacificationPost, ApiResponse>
    {
        private IReadGenericRepo<HiringPost> _RPR;
        private ISpecification<PublicTrip> _spec;

        public HiringPostQueryHandler(IReadGenericRepo<HiringPost> rPR)
        {
            _RPR = rPR;
        }


        public async Task<ApiResponse> Handle(GetHiringPostByTitle request, CancellationToken cancellationToken)
        {
            var posts = await _RPR.GetAll()
                 .Where(p => p.Title.Contains(request.Title))
                 .Include(p => p.Comments)
                     .ThenInclude(c => c.User)
                 .Include(p => p.CreatedBy)
                     .ThenInclude(p => p.User)
                 .OrderByDescending(p => p.CreatedAt)
                 .Select(p => new HiringPostTemplate
                 {
                     CreatedAt = p.CreatedAt,
                     FullName = $"{p.CreatedBy.User.FName} {p.CreatedBy.User.LName}",
                     Description = p.Description,
                     PhotoUrl = p.PhotoUrl,
                     Requirements = p.Requirements,
                     Status = p.Status,
                     Title = p.Title,
                     Comments = p.Comments.Select(c => new TemplateComment
                     {
                         CreatedAt = c.CreatedAt,
                         FullName = $"{c.User.FName} {c.User.LName}",
                         IsEdited = c.IsEdited,
                         Msg = c.Msg,
                     }).ToList()
                 }).ToListAsync(); // i need to fetch the last posts before 48h

            if (posts.Any())
            {
                return new ApiResponse(404);
            }
            return new ApiResultResponse<List<HiringPostTemplate>>(200, posts);
        }

        public async Task<ApiResponse> Handle(GetHiringPost request, CancellationToken cancellationToken)
        {
            var posts = await _RPR.GetAll()
                        .Where(p => p.CreatedAt >= request.Date)
                        .Include(p => p.Comments)
                            .ThenInclude(c => c.User)
                        .Include(p => p.CreatedBy)
                            .ThenInclude(p => p.User)
                        .OrderByDescending(p => p.CreatedAt)
                        .Select(p => new HiringPostTemplate
                        {
                            CreatedAt = p.CreatedAt,
                            FullName = $"{p.CreatedBy.User.FName} {p.CreatedBy.User.LName}",
                            Description = p.Description,
                            PhotoUrl = p.PhotoUrl,
                            Requirements = p.Requirements,
                            Status = p.Status,
                            Title = p.Title,
                            Comments = p.Comments.Select(c => new TemplateComment
                            {
                                CreatedAt = c.CreatedAt,
                                FullName = $"{c.User.FName} {c.User.LName}",
                                IsEdited = c.IsEdited,
                                Msg = c.Msg,
                            }).ToList()
                        }).ToListAsync(); // i need to fetch the last posts before 48h

            if (posts.Any())
            {
                return new ApiResponse(404);
            }
            return new ApiResultResponse<List<HiringPostTemplate>>(200, posts);
        }

        public async Task<ApiResponse> Handle(GetHiringSpacificationPost request, CancellationToken cancellationToken)
        {
            try
            {
                var spec = new HiringPostSearchSpecification(request.Date, request.Title,request.page,request.capacity);

                var posts = await _RPR
                    .GetAllSpec(spec)
                    .Select(x => new HiringPostTemplate
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        FullName = $"{x.CreatedBy.User.FName} {x.CreatedBy.User.LName}",
                        PhotoUrl = x.PhotoUrl,
                        Requirements = x.Requirements,
                        Status = x.Status,
                        CreatedAt = x.CreatedAt,
                        Comments = x.Comments.Select(c => new TemplateComment
                        {
                            CreatedAt = c.CreatedAt,
                            FullName = $"{c.User.FName} {c.User.LName}",
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
    
    internal class ExperiencePostQueryHandler : IQueryHandler<GetExperiencePostByTitle, ApiResponse>,
        IQueryHandler<GetExperienceSpacificationPost, ApiResponse>,
        IQueryHandler<GetExperiencePost, ApiResponse>
    {
        IReadGenericRepo<ExperiencePost> _RPR;

        public ExperiencePostQueryHandler(IReadGenericRepo<ExperiencePost> rPR)
        {
            _RPR = rPR;
        }

        public async Task<ApiResponse> Handle(GetExperiencePostByTitle request, CancellationToken cancellationToken)
        {
            var posts = await _RPR.GetAll()
                        .Where(p => p.Title.Contains(request.Title))
                        .Include(p => p.Comments)
                            .ThenInclude(c => c.User)
                        .Include(p => p.CreatedBy)
                        .OrderByDescending(p => p.CreatedAt)
                        .Order().Select(p => new ExperiencePostTemplate
                        {
                            CreatedAt = p.CreatedAt,
                            FullName = $"{p.CreatedBy.FName} {p.CreatedBy.LName}",
                            Description = p.Description,
                            PhotoUrl = p.PhotoUrl,
                            Title = p.Title,
                            City = p.City,
                            Country = p.Country,
                            Budget = p.Budget,
                            TipsAndRecommendations = p.TipsAndRecommendations,
                            Comments = p.Comments.Select(c => new TemplateComment
                            {
                                CreatedAt = c.CreatedAt,
                                FullName = $"{c.User.FName} {c.User.LName}",
                                IsEdited = c.IsEdited,
                                Msg = c.Msg,
                            }).ToList()
                        }).ToListAsync(); // i need to fetch the last posts before 48h

            if (posts.Any())
            {
                return new ApiResponse(404);
            }
            return new ApiResultResponse<List<ExperiencePostTemplate>>(200, posts);
        }

        public async Task<ApiResponse> Handle(GetExperiencePost request, CancellationToken cancellationToken)
        {
            var posts = await _RPR.GetAll()
                        .Where(p => p.CreatedAt >= request.Date)
                        .Include(p => p.Comments)
                            .ThenInclude(c => c.User)
                        .Include(p => p.CreatedBy)
                        .OrderByDescending(p => p.CreatedAt)
                        .Select(p => new ExperiencePostTemplate
                        {
                            CreatedAt = p.CreatedAt,
                            FullName = $"{p.CreatedBy.FName} {p.CreatedBy.LName}",
                            Description = p.Description,
                            PhotoUrl = p.PhotoUrl,
                            Title = p.Title,
                            City = p.City,
                            Country = p.Country,
                            Budget = p.Budget,
                            TipsAndRecommendations = p.TipsAndRecommendations,
                            Comments = p.Comments.Select(c => new TemplateComment
                            {
                                CreatedAt = c.CreatedAt,
                                FullName = $"{c.User.FName} {c.User.LName}",
                                IsEdited = c.IsEdited,
                                Msg = c.Msg,
                            }).ToList()
                        }).ToListAsync(); // i need to fetch the last posts before 48h

            if (posts.Any())
            {
                return new ApiResponse(404);
            }
            return new ApiResultResponse<List<ExperiencePostTemplate>>(200, posts);
        }

        public async Task<ApiResponse> Handle(GetExperienceSpacificationPost request, CancellationToken cancellationToken)
        {
            try
            {
                var spec = new ExperiencePostSearchSpecification(request.date, request.title, request.country,
                    request.city,request.tipsAndRecommendations,request.budget,request.page,request.capacity);

                var posts = await _RPR
                    .GetAllSpec(spec)
                    .Select(x => new ExperiencePostTemplate
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        FullName = $"{x.CreatedBy.FName} {x.CreatedBy.LName}",
                        PhotoUrl = x.PhotoUrl,
                        Budget = x.Budget,
                        City = x.City,
                        Country = x.Country,
                        TipsAndRecommendations = x.TipsAndRecommendations,
                        CreatedAt = x.CreatedAt,
                        Comments = x.Comments.Select(c => new TemplateComment
                        {
                            CreatedAt = c.CreatedAt,
                            FullName = $"{c.User.FName} {c.User.LName}",
                            IsEdited = c.IsEdited,
                            Msg = c.Msg,
                        }).ToList()
                    })
                    .ToListAsync();

                if (posts.Any())
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
