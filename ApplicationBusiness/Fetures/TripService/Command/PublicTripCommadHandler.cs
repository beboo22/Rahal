using Application.Abstraction.message;
using ApplicationBusiness.Fetures.BookingTripService.Command.Models;
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
    public record AddTourguideToPubTrip(int TourGuideId,int pubTripId):ICommand<ApiResponse>;
    public class PublicTripCommadHandler : ICommandHandler<AddPublicTrip, ApiResponse>,
                                    ICommandHandler<DeletePublicTrip, ApiResponse>,
                                    ICommandHandler<AddTourguideToPubTrip, ApiResponse>,
                                    ICommandHandler<CheckPubTripExsist, ApiResponse>


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
                    From = request.dto.From,
                    Title = request.dto.Title,
                    Destination = request.dto.Destination,
                    CreatedById = request.CreatedById,
                    StartDate = request.dto.StartDate,
                    IncludedPackages = (Package)request.dto.IncludedPackages,
                    TripCategory = request.dto.TripCategory,
                    MaxNumberOfMember = request.dto.NumberOfMember,
                    Duration = request.dto.Duration, // تأكد من إضافة الـ Duration

                    // تحويل الـ Activities الـ DTO لـ Entity مع البيانات الجديدة
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

                        // ربط بيانات الـ Option المختار
                        PlaceId = a.PlaceId,
                        DataId = a.DataId,
                        ActivityType = a.ActivityType,
                        Latitude = a.Latitude,
                        Longitude = a.Longitude,
                        Address = a.Address,
                        Thumbnail = a.Thumbnail, // الصورة اللي جاية من جوجل
                        Website = a.Website,
                        Phone = a.Phone,
                        Rating = a.Rating,
                        Reviews = a.Reviews,
                        PriceRange = a.PriceRange,
                        Description = a.Description,
                        serviceOption = string.Join(",", a.serviceOption)
                    }).ToList(),
                };

                // حساب الأسعار والعمولات
                var totalPrice = trip.PublicActivities.Sum(a => a.FullPrice);
                trip.Price = totalPrice;
                trip.TravelerFee = totalPrice * 0.01m;
                trip.OwnerTripFee = totalPrice * 0.05m;

                await _uot.BeginTransactionAsync();
                await _wTRepo.AddAsync(trip);
                await _uot.SaveChangesAsync();
                await _uot.CommitAsync();

                // الـ Mapping للـ Template (Response)
                // ... (نفس الكود القديم مع التأكد من نقل الـ Id الجديد)
                // 5. تجهيز الـ Template Trip للـ Response (لإرجاع الداتا للـ UI)
                var temp = new TemplateTrip
                {
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
                        Image = a.Thumbnail ?? a.Image, // الأولوية لصورة جوجل في العرض
                        TripCategory = a.TripCategory,
                        // يمكنك إضافة الـ Latitude والـ Longitude هنا لو الـ Front-end محتاج يرسم الخريطة فوراً
                        Latitude = a.Latitude,
                        Longitude = a.Longitude,
                        Description = a.Description,
                        ActivityType = a.ActivityType,
                        serviceOption = a.serviceOption.Split(",").ToList(),
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
                // تراجع عن العملية في حالة حدوث أي خطأ
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

                var tourguide = await Sender.Send(new CheckTourguideExsist(request.TourGuideId));
                if (tourguide.statusCode != StatusCodes.Status302Found)
                    return new ApiResponse(StatusCodes.Status404NotFound, "tourgiude not found");
                var trip = await _rPTR.GetByIdAsync(request.pubTripId);
                if (trip is null)
                    return new ApiResponse(StatusCodes.Status404NotFound, "trip not found");
                trip.TourGuideId = request.TourGuideId;
                await _uot.BeginTransactionAsync();
                await _wTRepo.UpdateAsync(trip, trip.Id);
                await _uot.SaveChangesAsync();
                await _uot.CommitAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                await _uot.RollbackAsync();
                return new ApiResponse(500, ex.Message);
            }



        }
    }

}
