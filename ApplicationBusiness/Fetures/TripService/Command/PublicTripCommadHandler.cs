using Application.Abstraction.message;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Fetures.Authentication.Query;
using ApplicationBusiness.Fetures.BookingTripService.Command.Models;
using ApplicationBusiness.Fetures.BookingTripService.Query;
using ApplicationBusiness.Fetures.Profile.Command.Models;
using ApplicationBusiness.Fetures.TripService.Command.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TripEntity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.TripService.Command
{

    public record AddTourguideToPubTrip(int TourGuideId, int pubTripId) : ICommand<ApiResponse>;
    public record UpdatePubTripStatus(int createdby,int pubTripId,TripStatus TripStatus) : ICommand<ApiResponse>;
    public class PublicTripCommadHandler : ICommandHandler<AddPublicTrip, ApiResponse>,
                                    ICommandHandler<DeletePublicTrip, ApiResponse>,
                                    ICommandHandler<AddTourguideToPubTrip, ApiResponse>,
                                    ICommandHandler<CheckPubTripExsist, ApiResponse>,
                                    ICommandHandler<UpdatePubTripStatus, ApiResponse>


    {
        private IWritepubTripRepo _wTRepo;
        private IWriteGenericRepo<BookingPublicTrip> _wBTRepo;
        private IWriteGenericRepo<User> _wURepo;



        private IReadGenericRepo<BookingPublicTrip> _rBTRepo;
        private IReadGenericRepo<PublicTrip> _rPTR;
        private IWriteUnitOfWork _uot;


        public ISender Sender { get; set; }


        public PublicTripCommadHandler(IWriteUnitOfWork wUnitOfWork,
            IWritepubTripRepo wTRepo,
            IReadGenericRepo<BookingPublicTrip> rBTRepo,
            IWriteGenericRepo<BookingPublicTrip> wBTRepo,
            IWriteGenericRepo<User> wURepo,
            ISender sender,
            IReadGenericRepo<PublicTrip> rPTR)
        {
            _uot = wUnitOfWork;
            _wTRepo = wTRepo;
            _rBTRepo = rBTRepo;
            _wBTRepo = wBTRepo;
            _wURepo = wURepo;
            Sender = sender;
            _rPTR = rPTR;
        }
        public async Task<ApiResponse> Handle(AddPublicTrip request, CancellationToken cancellationToken)
        {
            try
            {
                var checkUser = await _wURepo.ExistsAsync(request.CreatedById);
                if (!checkUser)
                    return new ApiResponse((int)HttpStatusCode.NotFound, "User not found");

                var trip = new PublicTrip()
                {
                    TripStatus = TripStatus.Upcoming, // تعيين الحالة الافتراضية للرحلة
                    From = request.dto.From,
                    Title = request.dto.Title,
                    Destination = request.dto.Destination,
                    CreatedById = request.CreatedById,
                    StartDate = request.dto.StartDate,
                    IncludedPackages = (Package)request.dto.IncludedPackages,
                    TripCategory = request.dto.TripCategory,
                    MaxNumberOfMember = request.dto.NumberOfMember,
                    Duration = request.dto.Duration,
                    CreatedAt = DateTime.UtcNow,

                    // قيم افتراضية يتم تعديلها بالأسفل بناءً على الصلاحيات
                    Price = 0,
                    TravelerFee = 0,
                    OwnerTripFee = 0,

                    PublicActivities = request.dto.Activities.Select(a => new ActivityPublicTrip
                    {
                        Title = a.Title,
                        Destination = a.Destination,
                        FullPrice = a.FullPrice,
                        SelectedDay = a.SelectedDay,
                        StartAt = a.StartAt,
                        EndAt = a.EndAt,
                        Image = a.Thumbnail ?? a.Image,
                        TripCategory = a.TripCategory,
                        CreatedAt = DateTime.UtcNow,
                        PlaceId = a.PlaceId,
                        DataId = a.DataId,
                        ActivityType = a.ActivityType,
                        Latitude = a.Latitude,
                        Longitude = a.Longitude,
                        Address = a.Address,
                        Thumbnail = a.Thumbnail,
                        Website = a.Website,
                        Phone = a.Phone,
                        Rating = a.Rating,
                        Reviews = a.Reviews,
                        PriceRange = a.PriceRange,
                        Description = a.Description,
                        serviceOption = a.serviceOption != null ? string.Join(",", a.serviceOption) : string.Empty
                    }).ToList(),
                };

                var rolesResponse = await Sender.Send(new GetRoleForUserById(request.CreatedById)) as ApiResultResponse<List<List<string>>>;

                // تعديل الشرط إلى && لحماية الكود من الـ NullReferenceException
                if (rolesResponse?.Data != null && rolesResponse.Data.Any(innerList => innerList.Contains(RoleEnum.TourGuide.ToString())))
                {
                    trip.TourGuideId = request.CreatedById;
                    var tourguide = await Sender.Send(new GetUserById(request.CreatedById)) as ApiResultResponse<TemplateGenericProfile>;

                    if (tourguide?.Data?.TourGuide != null)
                    {
                        var totalPrice = tourguide.Data.TourGuide.SalaryPerDay * trip.Duration;
                        trip.Price = totalPrice;
                        trip.TravelerFee = totalPrice * 0.01m;
                        trip.OwnerTripFee = totalPrice * 0.05m;
                    }
                }


                await _uot.BeginTransactionAsync();
                await _wTRepo.AddAsync(trip);
                await _uot.SaveChangesAsync();
                await _uot.CommitAsync();

                // تجهيز الـ Template للـ Response
                var temp = new TemplateTrip
                {
                    
                    NumberOfMember = trip.MaxNumberOfMember,
                    TravelerFee = trip.TravelerFee,
                    TourGuideId = request.dto.TourGuideId,
                    CreatedById = trip.CreatedById,
                    Id = trip.Id,
                    Title = trip.Title,
                    From = trip.From,
                    Destination = trip.Destination,
                    Duration = trip.Duration,
                    Price = trip.Price,
                    TripCategory = trip.TripCategory,
                    StartDate = trip.StartDate,
                    IncludedPackages = Enum.GetValues(typeof(Package)).Cast<Package>()
                                        .Where(p => p != Package.None && trip.IncludedPackages.HasFlag(p))
                                        .Select(p => (int)p)
                                        .ToList(),
                    Activities = trip.PublicActivities.Select(a => new TemplateActivity
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Destination = a.Destination,
                        FullPrice = a.FullPrice,
                        SelectedDay = a.SelectedDay,
                        StartAt = a.StartAt,
                        EndAt = a.EndAt,
                        Image = a.Image,
                        TripCategory = a.TripCategory,
                        Latitude = a.Latitude,
                        Longitude = a.Longitude,
                        Description = a.Description,
                        ActivityType = a.ActivityType,
                        // حماية الـ Split من الـ Null أو الفراغ
                        serviceOption = !string.IsNullOrEmpty(a.serviceOption) ? a.serviceOption.Split(",").ToList() : new List<string>(),
                        Address = a.Address,
                        Phone = a.Phone,
                        PlaceId = a.PlaceId,
                        DataId = a.DataId,
                        PriceRange = a.PriceRange,
                        Rating = a.Rating,
                        Reviews = a.Reviews,
                        Website = a.Website,
                    }).ToList()
                };

                return new ApiResultResponse<TemplateTrip>((int)HttpStatusCode.Created, temp, "Trip Added Successfully with all activities details.");
            }
            catch (Exception ex)
            {
                await _uot.RollbackAsync();
                return new ApiResponse(500, $"Internal Server Error: {ex.Message}");
            }
        }
        public async Task<ApiResponse> Handle(DeletePublicTrip request, CancellationToken cancellationToken)
        {
            try
            {
                //ensure user is the creator
                var item = await _rPTR.GetAll().Include(x => x.BookingPublicTrips).FirstOrDefaultAsync(x => x.Id == request.Id);

                if (item.CreatedById != request.createdBy)
                    return new ApiResponse(403);



                //var role = await Sender.Send(new GetRoleofUser(item.CreatedById)) as ApiResultResponse<List<RoleEnum>>;




                if (request.roles.Contains(RoleEnum.TourGuide))
                {

                    if (item.StartDate != null)
                        if (item.StartDate < DateTime.UtcNow)
                            if (_rBTRepo.GetAll().Any(x => x.PublicTripId == request.Id))
                                return new ApiResponse(403, "can't delete trip bec. there's booking and start date gone");
                            else
                                //return money
                                await Sender.Send(new ReturnMonyToUser(request.Id));
                }
                //delete trip
                await _uot.BeginTransactionAsync();
                await _wTRepo.DeleteAsync(request.Id); // always delete trip
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

        public async Task<ApiResponse> Handle(CheckPubTripExsist request, CancellationToken cancellationToken)
        {

            if (await _wTRepo.ExistsAsync(request.TripId))
                return new ApiResponse(StatusCodes.Status302Found);
            return new ApiResponse(StatusCodes.Status404NotFound);
        }

        public async Task<ApiResponse> Handle(AddTourguideToPubTrip request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. التأكد من وجود المرشد السياحي أولاً
                var tourguideResponse = await Sender.Send(new CheckTourguideExsist(request.TourGuideId));
                if (tourguideResponse.statusCode != StatusCodes.Status302Found)
                    return new ApiResponse(StatusCodes.Status404NotFound, "Tour guide not found");

                // 2. جلب الرحلة من ريبوزيتوري الكتابة لضمان الـ Tracking وتجنب مشاكل الـ DbContext
                var trip = await _rPTR.GetByIdAsync(request.pubTripId);
                if (trip is null)
                    return new ApiResponse(StatusCodes.Status404NotFound, "Trip not found");

                // 3. جلب بيانات المرشد السياحي للحصول على راتبه اليومي لإعادة حساب السعر
                var tourguideData = await Sender.Send(new GetUserById(request.TourGuideId)) as ApiResultResponse<TemplateGenericProfile>;
                if (tourguideData?.Data?.TourGuide == null)
                    return new ApiResponse(StatusCodes.Status404NotFound, "Tour guide profile details not found");

                // 4. ربط المرشد وإعادة حساب الأسعار بناءً على الـ Duration
                trip.TourGuideId = request.TourGuideId;

                var totalPrice = tourguideData.Data.TourGuide.SalaryPerDay * trip.Duration;
                trip.Price = totalPrice;
                trip.TravelerFee = totalPrice * 0.01m;
                trip.OwnerTripFee = totalPrice * 0.05m;

                // 5. حفظ التعديلات داخل Transaction آمنة
                await _uot.BeginTransactionAsync();

                await _wTRepo.UpdateAsync(trip, trip.Id);
                await _uot.SaveChangesAsync();

                await _uot.CommitAsync();

                return new ApiResponse(StatusCodes.Status200OK, "Tour guide added and trip prices updated successfully.");
            }
            catch (Exception ex)
            {
                // لتجنب ضرب الكود لو الـ Transaction لم تبدأ بعد
                try
                {
                    await _uot.RollbackAsync();
                }
                catch { /* تجاهل خطأ الـ rollback لو الـ transaction مكنتش بدأت أساساً */ }

                return new ApiResponse(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> Handle(UpdatePubTripStatus request, CancellationToken cancellationToken)
        {
            var trip = await _rPTR.GetByIdAsync(request.pubTripId);
            if (trip == null)
                return new ApiResponse(StatusCodes.Status404NotFound, "Trip not found");
            if(request.createdby != trip.CreatedById)
                return new ApiResponse(StatusCodes.Status403Forbidden, "You are not authorized to update this trip");
            var CheakBooking = await Sender.Send(new IsBookingExistToTrip(request.pubTripId)) as ApiResultResponse<bool>;
            if (CheakBooking != null && CheakBooking.Data&&request.TripStatus == TripStatus.Cancelled)
                return new ApiResponse(StatusCodes.Status403Forbidden, "You can't cancel this trip because there are bookings for it");
            trip.TripStatus = request.TripStatus;
            try
            {

                await _uot.BeginTransactionAsync();
                await _wTRepo.UpdateAsync(trip, trip.Id);
                await _uot.SaveChangesAsync();
                await _uot.CommitAsync();
                return new ApiResponse(StatusCodes.Status200OK, "Trip status updated to WaitingForGuideApproval successfully.");
            }
            catch (Exception ex)
            {
                try
                {
                    await _uot.RollbackAsync();
                }
                catch { /* تجاهل خطأ الـ rollback لو الـ transaction مكنتش بدأت أساساً */ }
                return new ApiResponse(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");

            }
        }
    }
}


