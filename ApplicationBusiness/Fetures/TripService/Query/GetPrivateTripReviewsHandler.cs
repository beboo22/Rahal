using Application.Abstraction.message;
using ApplicationBusiness.Abstraction;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.TripEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.TripService.Query
{
    public record GetPrivateTripReviews(int TripId)
    : IQuery<ApiResponse>;
    internal class GetPrivateTripReviewsHandler
    : IQueryHandler<GetPrivateTripReviews, ApiResponse>
    {
        private readonly IReadGenericRepo<ReviewPrivateTrip> _repo;
        private readonly IReviewQueryService _service;

        public GetPrivateTripReviewsHandler(
            IReadGenericRepo<ReviewPrivateTrip> repo,
            IReviewQueryService service)
        {
            _repo = repo;
            _service = service;
        }

        public async Task<ApiResponse> Handle(
            GetPrivateTripReviews request,
            CancellationToken cancellationToken)
        {
            var reviews = _repo.GetAll().Include(x => x.Reviewer)
                                            .ThenInclude(reviews => reviews.TravelerProfile)
                                        .Include(reviews => reviews.Reviewer)
                                            .ThenInclude(reviews => reviews.TourGuideProfile)
                                        .Include(reviews => reviews.Reviewer)
                                            .ThenInclude(reviews => reviews.TravelerCompanyProfile)
                .Where(x => x.PrivateTripId == request.TripId);

            var response = _service.BuildResponse(reviews);

            return new ApiResultResponse<ReviewsResponseDto>(200, response);
        }
    }
}
    