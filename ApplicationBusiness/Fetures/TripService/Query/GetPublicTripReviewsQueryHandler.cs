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
    public class ReviewDto
    {
        public int ReviewerId { get; set; }

        public decimal Rating { get; set; }

        public string Feedback { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class ReviewResponseDto
    {
        public decimal AverageRate { get; set; }

        public int TotalReviews { get; set; }

        public List<ReviewDto> Reviews { get; set; } = [];
    }
    public record GetPublicTripReviews(int TripId)
    : IQuery<ApiResponse>;
    internal class GetPublicTripReviewsHandler
        : IQueryHandler<GetPublicTripReviews, ApiResponse>
    {
        private readonly IReadGenericRepo<ReviewPublicTrip> _repo;
        private readonly IReviewQueryService _service;

        public GetPublicTripReviewsHandler(
            IReadGenericRepo<ReviewPublicTrip> repo,
            IReviewQueryService service)
        {
            _repo = repo;
            _service = service;
        }

        public async Task<ApiResponse> Handle(
            GetPublicTripReviews request,
            CancellationToken cancellationToken)
        {
            var reviews = _repo.GetAll().Include(x => x.Reviewer)
                                            .ThenInclude(reviews => reviews.TravelerProfile)
                                        .Include(reviews => reviews.Reviewer)
                                            .ThenInclude(reviews => reviews.TourGuideProfile)
                                        .Include(reviews => reviews.Reviewer)
                                            .ThenInclude(reviews => reviews.TravelerCompanyProfile)
                .Where(x => x.PublicTripId == request.TripId);

            var response = _service.BuildResponse(reviews);

            return new ApiResultResponse<ReviewsResponseDto>(200, response);
        }
    }
}
