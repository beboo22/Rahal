using Application.Abstraction.message;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.TripService.Command
{
    public record AddReviewToTourGuide(
    AddReviewTourGuideDto dto,
    int UserId) : ICommand<ApiResponse>;

    public class AddReviewTourGuideDto
    {

        public int TourGuideId { get; set; }
        public string Feedback { get; set; }
        [Range(0, 5)]
        public decimal Rate { get; set; }
    }

    internal class ReviewTourGuideCommandHandler
    : ICommandHandler<AddReviewToTourGuide, ApiResponse>
    {
        private readonly IWriteUnitOfWork _uow;
        private readonly IWriteGenericRepo<Review> _wrr;
        private readonly IWriteGenericRepo<TourGuide> _tourGuideRepo;

        public ReviewTourGuideCommandHandler(
            IWriteUnitOfWork uow,
            IWriteGenericRepo<Review> wrr,
            IWriteGenericRepo<TourGuide> tourGuideRepo)
        {
            _uow = uow;
            _wrr = wrr;
            _tourGuideRepo = tourGuideRepo;
        }

        public async Task<ApiResponse> Handle(
            AddReviewToTourGuide request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransactionAsync();

                var tourGuide =
                    await _tourGuideRepo.ExistsAsync(request.dto.TourGuideId);

                if (tourGuide is false)
                    return new ApiResponse(404, "Tour Guide Not Found");

                var review = new ReviewTourGuide
                {
                    TourGuideId = request.dto.TourGuideId,
                    ReviewerId = request.UserId,
                    Rating = request.dto.Rate,
                    Feedback = request.dto.Feedback
                };

                await _wrr.AddAsync(review);

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                return new ApiResponse(201, "Successfully Created");
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return new ApiResponse(500,
                    ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
