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
    public record GetTourGuideReviews(int TourGuideId)
    : IQuery<ApiResponse>;
    internal class GetTourGuideReviewsHandler
    : IQueryHandler<GetTourGuideReviews, ApiResponse>
    {
        private readonly IReadGenericRepo<ReviewTourGuide> _repo;
        private readonly IReviewQueryService _service;

        public GetTourGuideReviewsHandler(
            IReadGenericRepo<ReviewTourGuide> repo,
            IReviewQueryService service)
        {
            _repo = repo;
            _service = service;
        }

        public async Task<ApiResponse> Handle(
            GetTourGuideReviews request,
            CancellationToken cancellationToken)
        {
            var reviews = _repo.GetAll().Include(x => x.Reviewer)
                                            .ThenInclude(reviews => reviews.TravelerProfile)
                                        .Include(reviews => reviews.Reviewer)
                                            .ThenInclude(reviews => reviews.TourGuideProfile)
                                        .Include(reviews => reviews.Reviewer)
                                            .ThenInclude(reviews => reviews.TravelerCompanyProfile)
                .Where(x => x.TourGuideId == request.TourGuideId);

            var response = _service.BuildResponse(reviews);

            return new ApiResultResponse<ReviewsResponseDto>(200, response);
        }
    }
}
