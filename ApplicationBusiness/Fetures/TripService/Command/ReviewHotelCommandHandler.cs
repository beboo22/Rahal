using Application.Abstraction.message;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.TripService.Command
{
    public record AddReviewToHotel(
    AddReviewHotelDto dto,
    int UserId) : ICommand<ApiResponse>;

    public class AddReviewHotelDto
    {
        public int HotelId { get; set; }
        public string Feedback { get; set; }
        [Range(0, 5)]
        public decimal Rate { get; set; }
    }

    internal class ReviewHotelCommandHandler
    : ICommandHandler<AddReviewToHotel, ApiResponse>
    {
        private readonly IWriteUnitOfWork _uow;
        private readonly IWriteGenericRepo<Review> _wrr;
        private readonly IWriteGenericRepo<Hotel> _hotelRepo;

        public ReviewHotelCommandHandler(
            IWriteUnitOfWork uow,
            IWriteGenericRepo<Review> wrr,
            IWriteGenericRepo<Hotel> hotelRepo)
        {
            _uow = uow;
            _wrr = wrr;
            _hotelRepo = hotelRepo;
        }

        public async Task<ApiResponse> Handle(
            AddReviewToHotel request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransactionAsync();

                var hotel =
                    await _hotelRepo.ExistsAsync(request.dto.HotelId);

                if (hotel is false)
                    return new ApiResponse(404, "Hotel Not Found");

                var review = new ReviewHotel
                {
                    HotelId = request.dto.HotelId,
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
