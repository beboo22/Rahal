using Application.Abstraction.message;
using ApplicationBusiness.Fetures.likesSerive.Query.Models;
using ApplicationBusiness.Fetures.PostService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.PostEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.likesSerive.Query
{
    internal class LikesQueryHandler : IQueryHandler<GetUserLikeToPost, ApiResponse>
    {
        private IReadGenericRepo<Likes> _rRepo;

        public LikesQueryHandler(IReadGenericRepo<Likes> rRepo)
        {
            _rRepo = rRepo;
        }

        public async Task<ApiResponse> Handle(GetUserLikeToPost request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _rRepo.GetAll()
                    .Where(x => x.postId == request.postId)
                    // No need for .Include() when using .Select(), EF handles the join automatically
                    .Select(x => new TemplateuserLikePost
                    {
                        // In EF Core, string interpolation or + is preferred over .Concat

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
                        },
                        LikeType = x.LikeType
                    })
                    .ToListAsync(cancellationToken);

                return new ApiResultResponse<List<TemplateuserLikePost>>(
                200, result,
                    "Users retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error fetching likes: {ex.Message}");
            }
        }
    }
}
