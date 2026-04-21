using Application.Abstraction.message;
using ApplicationBusiness.Fetures.TripService.Command.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TripEntity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ApplicationBusiness.Fetures.TripService.Command
{

    public class PublicTripCommadHandler : ICommandHandler<AddPublicTrip, ApiResponse>,
                                    ICommandHandler<DeletePublicTrip, ApiResponse>
                                    
    {
        private IWriteGenericRepo<PublicTrip> _wTRepo;
        private IWriteGenericRepo<BookingPublicTrip> _wBTRepo;
        private IWriteGenericRepo<User> _wURepo;



        private IReadGenericRepo<BookingPublicTrip> _rBTRepo;
        private IWriteUnitOfWork _uot;

        public PublicTripCommadHandler(IWriteUnitOfWork wUnitOfWork, IWriteGenericRepo<PublicTrip> wTRepo, IReadGenericRepo<BookingPublicTrip> rBTRepo, IWriteGenericRepo<BookingPublicTrip> wBTRepo, IWriteGenericRepo<User> wURepo)
        {
            _uot = wUnitOfWork;
            _wTRepo = wTRepo;
            _rBTRepo = rBTRepo;
            _wBTRepo = wBTRepo;
            _wURepo = wURepo;
        }

        public async Task<ApiResponse> Handle(AddPublicTrip request, CancellationToken cancellationToken)
        {
            await _uot.BeginTransactionAsync();
            try
            {
                var checkUser = await _wURepo.ExistsAsync(request.CreatedById);
                if (!checkUser)
                {
                    return new ApiResponse((int)HttpStatusCode.NotFound, "User not found");
                }
                var trip = new PublicTrip()
                {
                    From = request.dto.From,
                    Title = request.dto.Title,
                    Destination = request.dto.Destination,
                    CreatedById = request.CreatedById,
                    StartDate = request.dto.StartDate,
                    IncludedPackages = (Package)request.dto.IncludedPackages,
                    TripCategory = request.dto.TripCategory,
                    MaxNumberOfMember = request.dto.NumberOfMember,
                    PublicActivities = request.dto.Activities.Select(a => new ActivityPublicTrip
                    {
                        Destination = a.Destination,
                        FullPrice = a.FullPrice,
                        SelectedDay = a.SelectedDay,
                        CreatedAt = DateTime.UtcNow,
                        EndAt = a.EndAt,
                        Image = a.Image,
                        StartAt = a.StartAt,
                        Title = a.Title,
                        TripCategory = a.TripCategory,
                    }).ToList(),
                };
                var totalPrice = trip.PublicActivities.Sum(a => a.FullPrice);
                trip.Price = totalPrice;
                trip.TravelerFee = totalPrice * 0.01m; // 1 % fee for traveler
                trip.OwnerTripFee = totalPrice * 0.05m; // 5 % fee for trip owner
                await _wTRepo.AddAsync(trip);
                await _uot.SaveChangesAsync();
                await _uot.CommitAsync();
                var temp = new TemplateTrip
                {
                    Id = trip.Id,
                    Title = trip.Title,
                    From = trip.From,
                    Destination = trip.Destination,
                    Duration = trip.Duration,
                    Price = trip.Price,
                    TripCategory = trip.TripCategory,
                    IncludedPackages = trip.IncludedPackages,
                    NumberOfMember = trip.MaxNumberOfMember,
                    StartDate = trip.StartDate,
                    Activities = trip.PublicActivities.Select(a => new TemplateActivity
                    {
                        Id = a.Id,
                        Destination = a.Destination,
                        FullPrice = a.FullPrice,
                        SelectedDay = a.SelectedDay,
                        EndAt = a.EndAt,
                        Image = a.Image,
                        StartAt = a.StartAt,
                        Title = a.Title,
                        TripCategory = a.TripCategory,
                    }).ToList()
                };
                return new ApiResultResponse<TemplateTrip>((int)HttpStatusCode.Created, temp, "Trip Added Successfully");
            }
            catch (Exception ex)
            {
                await _uot.RollbackAsync();
                return new ApiResponse(500, ex.Message);
            }

        }

        public async Task<ApiResponse> Handle(DeletePublicTrip request, CancellationToken cancellationToken)
        {
            await _uot.BeginTransactionAsync();
            try
            {
                var bookings = _rBTRepo.GetAll()
                    .Include(b => b.User)
                    .Where(b => b.PublicTrip.Id == request.Id && b.IsPaid)
                    .ToList();

                foreach (var item in bookings)
                {
                    if(item.User.FinancialBalance.HasValue)
                        item.User.FinancialBalance += item.TotalBookingPrice;
                    else item.User.FinancialBalance = item.TotalBookingPrice;
                }

                // Build tasks dynamically
                var tasks = new List<Task>
                {
                    _wTRepo.DeleteAsync(request.Id) // always delete trip
                };

                if (bookings != null && bookings.Any())
                {
                    var users = bookings.Select(b => b.User).ToList();
                    tasks.Add(_wURepo.UpdateRangeAsync(users));
                }

                await Task.WhenAll(tasks);

                await _uot.SaveChangesAsync();
                await _uot.CommitAsync();

                return new ApiResponse(200, "Trip deleted successfully");
            }
            catch (Exception ex)
            {
                await _uot.RollbackAsync();
                return new ApiResponse(500, ex.Message);
            }
        }

    }


    public class PrivateTripCommadHandler : ICommandHandler<AddPrivateTrip, ApiResponse>,
                                    ICommandHandler<DeletePrivateTrip, ApiResponse>
    {
        private IWriteGenericRepo<PrivateTrip> _wTRepo;

        private IWriteGenericRepo<User> _wURepo;
        private IWriteUnitOfWork _uot;
        public PrivateTripCommadHandler(IWriteGenericRepo<PrivateTrip> wTRepo, IWriteUnitOfWork uot, IWriteGenericRepo<User> wURepo)
        {
            _wTRepo = wTRepo;
            _uot = uot;
            _wURepo = wURepo;
        }
        public async Task<ApiResponse> Handle(AddPrivateTrip request, CancellationToken cancellationToken)
        {
            await _uot.BeginTransactionAsync();
            try
            {
                var checkUser = await _wURepo.ExistsAsync(request.CreatedById);
                if (!checkUser)
                {
                    return new ApiResponse((int)HttpStatusCode.NotFound, "User not found");
                }
                var trip = new PrivateTrip()
                {
                    From = request.dto.From,
                    Title = request.dto.Title,
                    Destination = request.dto.Destination,
                    CreatedById = request.CreatedById,
                    StartDate = request.dto.StartDate,
                    TripCategory = request.dto.TripCategory,
                    PrivateActivities = request.dto.Activities.Select(a => new ActivityPrivateTrip
                    {
                        Destination = a.Destination,
                        FullPrice = a.FullPrice,
                        SelectedDay = a.SelectedDay,
                        CreatedAt = DateTime.UtcNow,
                        EndAt = a.EndAt,
                        Image = a.Image,
                        StartAt = a.StartAt,
                        Title = a.Title,
                        TripCategory = a.TripCategory,
                    }).ToList(),
                };
                var totalPrice = trip.PrivateActivities.Sum(a => a.FullPrice);
                trip.Price = totalPrice;
                trip.CustomizationFee = totalPrice * 0.05m;
                await _wTRepo.AddAsync(trip);
                await _uot.SaveChangesAsync();
                await _uot.CommitAsync();
                var temp = new PrivateTemplateTrip
                {
                    Id = trip.Id,
                    Title = trip.Title,
                    From = trip.From,
                    Destination = trip.Destination,
                    Duration = trip.Duration,
                    Price = trip.Price,
                    TripCategory = trip.TripCategory,
                    StartDate = trip.StartDate,
                    Activities = trip.PrivateActivities.Select(a => new TemplateActivity
                    {
                        Id = a.Id,
                        Destination = a.Destination,
                        FullPrice = a.FullPrice,
                        SelectedDay = a.SelectedDay,
                        EndAt = a.EndAt,
                        Image = a.Image,
                        StartAt = a.StartAt,
                        Title = a.Title,
                        TripCategory = a.TripCategory,
                    }).ToList()
                };
                return new ApiResultResponse<PrivateTemplateTrip>((int)HttpStatusCode.Created, temp, "Trip Added Successfully");

            }
            catch (Exception ex)
            {
                await _uot.RollbackAsync();
                return new ApiResponse(500, ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(DeletePrivateTrip request, CancellationToken cancellationToken)
        {
            await _uot.BeginTransactionAsync();
            try
            {
                await _wTRepo.DeleteAsync(request.Id);


                await _uot.SaveChangesAsync();
                await _uot.CommitAsync();

                return new ApiResponse(200, "Trip deleted successfully");
            }
            catch (Exception ex)
            {
                await _uot.RollbackAsync();
                return new ApiResponse(500, ex.Message);
            }
        }

    }


}
