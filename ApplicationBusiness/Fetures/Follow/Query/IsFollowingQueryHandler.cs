using ApplicationBusiness.Fetures.Follow.Query.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Follow.Query
{
    public class IsFollowingQueryHandler : IRequestHandler<IsFollowingQuery, ApiResponse>
    {
        private readonly IReadGenericRepo<UserFollow> _repo;

        public IsFollowingQueryHandler(IReadGenericRepo<UserFollow> repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse> Handle(IsFollowingQuery request, CancellationToken cancellationToken)
        {
            // البحث عن علاقة المتابعة
            var followRelation = await _repo.GetAll().FirstOrDefaultAsync(
                uf => uf.FollowerId == request.FollowerId &&
                      uf.FollowingId == request.FollowingId,
                cancellationToken);

            if (followRelation == null)
            {
                return new ApiResponse(
                    404,
                    "Follow relationship not found"
                );
            }

            // إرجاع الـ Id مع الرد
            return new ApiResultResponse<UserFollow>(
                200,
                followRelation,  // أو data: followRelation.Id
                "Follow relationship found"
            );
        }
    }
}
