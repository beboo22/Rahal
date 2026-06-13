using ApplicationBusiness.Fetures.TripService.Query;
using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Abstraction
{
    public interface IReviewQueryService
    {
        ReviewsResponseDto BuildResponse<TReview>(
            IQueryable<TReview> reviews)
            where TReview : Review;
    }
    public class ReviewDto
    {
        public Reviewer Reviewer { get; set; }

        public decimal Rating { get; set; }

        public string Feedback { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class Reviewer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string photoUrl { get; set; }
    }

    public class ReviewsResponseDto
    {
        public decimal AverageRate { get; set; }

        public int TotalReviews { get; set; }

        public List<ReviewDto> Reviews { get; set; } = [];
    }
    public class ReviewQueryService : IReviewQueryService
    {
        public ReviewsResponseDto BuildResponse<TReview>(
            IQueryable<TReview> reviews)
            where TReview : Review
        {
            var reviewList = reviews.ToList();

            return new ReviewsResponseDto
            {
                AverageRate = reviewList.Any()
                    ? reviewList.Average(x => x.Rating)
                    : 0,

                TotalReviews = reviewList.Count,

                Reviews = reviewList.Select(x => new ReviewDto
                {
                    Reviewer = new Reviewer
                    {
                        Id = x.ReviewerId ?? 0,
                        Name = x.Reviewer.FName + " " + x.Reviewer.LName,
                        photoUrl = x.Reviewer?.TravelerProfile is not null ?
                        x.Reviewer?.TravelerProfile.PhotoUrl : x.Reviewer?.TravelerCompanyProfile is not null?
                        x.Reviewer?.TravelerCompanyProfile.PhotoUrl : x.Reviewer?.TourGuideProfile is not null ? x.Reviewer?.TourGuideProfile.PhotoUrl : null
                    },
                    Rating = x.Rating,
                    Feedback = x.Feedback,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };  
        }
    }
}
