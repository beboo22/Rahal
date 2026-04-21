using Application.Abstraction.message;
using ApplicationBusiness.Fetures.TripService.Command.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.TripService.Command
{
    internal class ReviewPublicTripCommandHandler : ICommandHandler<AddReviewToPubliucTrip, ApiResponse>
    {
        private IWriteUnitOfWork _uow;
        private IWriteGenericRepo<Review> _wrr;
        private IReadGenericRepo<BookingPublicTrip> _rbr;
        private IReadGenericRepo<PublicTrip> _rTr;
        public ReviewPublicTripCommandHandler(IWriteUnitOfWork uow, IWriteGenericRepo<Review> wrr, IReadGenericRepo<BookingPublicTrip> rbr, IReadGenericRepo<PublicTrip> rTr)
        {
            _uow = uow;
            _wrr = wrr;
            _rbr = rbr;
            _rTr = rTr;
        }

        public async Task<ApiResponse> Handle(AddReviewToPubliucTrip request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                //Ensure that User Already Booking and The Trip is finished
                var checkBooking = _rbr.GetAll().Any(b => b.PublicTripId == request.dto.TripId && b.UserId == request.UserId);
                if (!checkBooking)
                    return new ApiResponse(404, "User don't have Booking");
                var checkTrip = _rTr.GetAll().FirstOrDefault(t => t.Id == request.dto.TripId);
                if (checkTrip?.StartDate.AddDays(checkTrip.Duration) > DateTime.UtcNow)
                    return new ApiResponse((int)HttpStatusCode.Forbidden, "should Make review After Finished the Trip");
                var review = new ReviewPublicTrip()
                {
                    PublicTripId = request.dto.TripId,
                    ReviewerId = request.UserId,
                    RevieweeId = checkTrip?.CreatedById,
                    Rating = request.dto.Rate,
                    Feedback = request.dto.Feedback,
                };
                await _wrr.AddAsync(review);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();
                return new ApiResponse(201, "successfully Create");
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return new ApiResponse(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
    internal class ReviewPrivateTripCommandHandler : ICommandHandler<AddReviewToPrivateTrip, ApiResponse>
    {
        private IWriteUnitOfWork _uow;

        public ReviewPrivateTripCommandHandler(IWriteUnitOfWork uow, IWriteGenericRepo<Review> wrr, IReadGenericRepo<PrivateTrip> rTr, IWriteGenericRepo<PrivateTrip> wTr)
        {
            _uow = uow;
            _wrr = wrr;
            _rTr = rTr;
            _wTr = wTr;
        }

        private IWriteGenericRepo<Review> _wrr;
        private IReadGenericRepo<PrivateTrip> _rTr;
        private IWriteGenericRepo<PrivateTrip> _wTr;
        public async Task<ApiResponse> Handle(AddReviewToPrivateTrip request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                var Trip = await _rTr.GetByIdAsync(request.dto.TripId);
                if (Trip is not null)
                    return new ApiResponse(404, "Not Found Trip");
                var check = Trip.CreatedById == request.UserId;
                if (check)
                    return new ApiResponse((int)HttpStatusCode.Forbidden, "u can't put review");

                check = Trip.StartDate.AddDays(Trip.Duration) > DateTime.Now;
                if (check)
                    return new ApiResponse((int)HttpStatusCode.Forbidden, "can't put review now");

                var review = new ReviewPrivateTrip()
                {
                    PrivateTripId = request.dto.TripId,
                    ReviewerId = request.UserId,
                    Rating = request.dto.Rate,
                    Feedback = request.dto.Feedback,
                };
                if (Trip?.TourGuideId is int tourGuideId)
                    review.RevieweeId = tourGuideId;
                await _wrr.AddAsync(review);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();
                return new ApiResponse(201, "successfully Create");
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return new ApiResponse(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
