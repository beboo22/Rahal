using Application.Abstraction.message;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Fetures.BookingTripService.Command.Models;
using ApplicationBusiness.Fetures.Profile.Command.Models;
using ApplicationBusiness.Fetures.TripService.Command.Models;
using ApplicationBusiness.Fetures.TripService.Query.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TripEntity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ApplicationBusiness.Fetures.TripService.Command
{
    public record AddTourguideToPivTrip(int TourGuideId, int pubTripId) : ICommand<ApiResponse>;

    public record UpdatePrivTripStatus(int createdby,int pubTripId, TripStatus TripStatus) : ICommand<ApiResponse>;

    public class PrivateTripCommadHandler : ICommandHandler<AddPrivateTrip, ApiResponse>,
                                    ICommandHandler<DeletePrivateTrip, ApiResponse>,
                                    ICommandHandler<AddTourguideToPivTrip, ApiResponse>,
                                    ICommandHandler<CheckPrivTripExsist, ApiResponse>,
                                    ICommandHandler<UpdatePrivTripStatus, ApiResponse>

    {
        private IWriteGenericRepo<PrivateTrip> _wTRepo;

        private IWriteGenericRepo<User> _wURepo;
        private IWriteUnitOfWork _uot;
        private IReadGenericRepo<PrivateTrip> _rPTR;


        public ISender Sender { get; set; }


        public PrivateTripCommadHandler(IWriteGenericRepo<PrivateTrip> wTRepo, IWriteUnitOfWork uot, IWriteGenericRepo<User> wURepo, ISender sender, IReadGenericRepo<PrivateTrip> rPTR)
        {
            _wTRepo = wTRepo;
            _uot = uot;
            _wURepo = wURepo;
            Sender = sender;
            _rPTR = rPTR;
        }
        public async Task<ApiResponse> Handle(AddPrivateTrip request, CancellationToken cancellationToken)
        {
            await _uot.BeginTransactionAsync();
            try
            {
                // 1. التاكد من وجود المستخدم
                var checkUser = await _wURepo.ExistsAsync(request.CreatedById);
                if (!checkUser)
                {
                    return new ApiResponse((int)HttpStatusCode.NotFound, "User not found");
                }

                // 2. إنشاء الكيان (Trip Entity) مع الـ Activities الملحقة بكل تفاصيلها
                var trip = new PrivateTrip()
                {
                    TripStatus = TripStatus.Upcoming, // الحالة الافتراضية عند الإنشاء
                    From = request.dto.From,
                    Title = request.dto.Title,
                    Destination = request.dto.Destination,
                    CreatedById = request.CreatedById,
                    StartDate = request.dto.StartDate,
                    TripCategory = request.dto.TripCategory,
                    Duration = request.dto.Duration, // تأكد من وجودها في الـ DTO
                    
                    // Mapping الـ Activities مع كل بيانات SerpApi
                    PrivateActivities = request.dto.Activities.Select(a => new ActivityPrivateTrip
                    {
                        Title = a.Title,
                        Destination = a.Destination,
                        FullPrice = a.FullPrice,
                        SelectedDay = a.SelectedDay,
                        StartAt = a.StartAt,
                        EndAt = a.EndAt,
                        Image = a.Image, // الصورة اليدوية إن وجدت
                        TripCategory = a.TripCategory,
                        CreatedAt = DateTime.UtcNow,

                        // --- بيانات Google Maps/SerpApi المضافة ---
                        PlaceId = a.PlaceId,
                        DataId = a.DataId,
                        ActivityType = a.ActivityType, // (Breakfast, Lunch, Mall, etc.)
                        Latitude = a.Latitude,
                        Longitude = a.Longitude,
                        Address = a.Address,
                        Thumbnail = a.Thumbnail, // صورة المكان من جوجل
                        Website = a.Website,
                        Phone = a.Phone,
                        Rating = a.Rating,
                        Reviews = a.Reviews,
                        PriceRange = a.PriceRange,
                        Description = a.Description,
                        serviceOption = string.Join(",", a.serviceOption)
                    }).ToList(),
                };

                // 3. حساب السعر الإجمالي والعمولة
                var totalPrice = trip.PrivateActivities.Sum(a => a.FullPrice);
                trip.Price = totalPrice;
                trip.CustomizationFee = totalPrice * 0.05m; // 5% عمولة تخصيص

                // 4. الحفظ في قاعدة البيانات
                await _wTRepo.AddAsync(trip);
                await _uot.SaveChangesAsync();
                await _uot.CommitAsync();

                // 5. تجهيز الـ Template Trip للـ Response (لإرجاع الداتا للـ UI)
                var temp = new PrivateTemplateTrip
                {
                    TourGuideId = trip.TourGuideId,
                    Id = trip.Id,
                    Title = trip.Title,
                    From = trip.From,
                    Destination = trip.Destination,
                    Duration = trip.Duration,
                    Price = trip.Price,
                    TripCategory = trip.TripCategory,
                    
                    Activities = trip.PrivateActivities.Select(a => new TemplateActivity
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
                        serviceOption = a.serviceOption?
                    .Split(',')
                    .ToList()
                ?? new List<string>(),
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

                return new ApiResultResponse<PrivateTemplateTrip>((int)HttpStatusCode.Created, temp, "Trip Added Successfully with all activities details.");
            }
            catch (Exception ex)
            {
                // تراجع عن العملية في حالة حدوث أي خطأ
                await _uot.RollbackAsync();
                return new ApiResponse(500, $"Internal Server Error: {ex.Message}");
            }
        }
        public async Task<ApiResponse> Handle(DeletePrivateTrip request, CancellationToken cancellationToken)
        {
            try
            {
                var item = await Sender.Send(new GetPrivTripSpecQuery(new TripFilter
                {
                    Id = request.Id,
                })) as ApiResultResponse<PrivateTemplateTrip>;
                if (item?.Data == null)
                    return new ApiResponse(404);

                if (item?.Data.CreatedById != request.createdBy)
                    return new ApiResponse(403);
                await _uot.BeginTransactionAsync();
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

        public async Task<ApiResponse> Handle(CheckPrivTripExsist request, CancellationToken cancellationToken)
        {

            if (await _wTRepo.ExistsAsync(request.TripId))
                return new ApiResponse(StatusCodes.Status302Found);
            return new ApiResponse(StatusCodes.Status404NotFound);
        }

        public async Task<ApiResponse> Handle(AddTourguideToPivTrip request, CancellationToken cancellationToken)
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

        public async Task<ApiResponse> Handle(UpdatePrivTripStatus request, CancellationToken cancellationToken)
        {
            try
            {
                var trip = await _rPTR.GetByIdAsync(request.pubTripId);
                if (trip is null)
                    return new ApiResponse(StatusCodes.Status404NotFound, "trip not found");
                trip.TripStatus = request.TripStatus;
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
