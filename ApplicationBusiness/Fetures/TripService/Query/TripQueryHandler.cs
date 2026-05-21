using Application.Abstraction.message;
using Application.Abstraction.Specification;
using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Fetures.TripService.Query.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.TripEntity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ApplicationBusiness.Fetures.TripService.Query
{
    public class PublicTripQueryHandler : 
                                         IQueryHandler<GetPubTripSpecQuery, ApiResponse>
    {
        private IReadGenericRepo<PublicTrip> _repo;
        private ISpecification<PublicTrip> _spec;
        public PublicTripQueryHandler(IReadGenericRepo<PublicTrip> repo, ISpecification<PublicTrip> spec)
        {
            _repo = repo;
            _spec = spec;
        }

        //public async Task<ApiResponse> Handle(SearchForTrip request, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        if (request?.dto == null)
        //        {
        //            return new ApiResponse
        //            (
        //                400,
        //                "Invalid request data"
        //            );
        //        }

        //        Expression<Func<PublicTrip, bool>>? criteria = null;

        //        if (!string.IsNullOrEmpty(request.dto.Title))
        //        {
        //            criteria = trip => trip.Title.Contains(request.dto.Title);
        //        }

        //        if (!string.IsNullOrEmpty(request.dto.Destination))
        //        {
        //            // Combine with previous condition if it exists
        //            if (criteria != null)
        //                criteria = criteria.AndAlso(trip => trip.Destination.Contains(request.dto.Destination));
        //            else
        //                criteria = trip => trip.Destination.Contains(request.dto.Destination);
        //        }

        //        _spec.crateria = criteria;

        //        // Example: Add pagination if provided in request.dto
        //        if (request.dto.PageSize > 0 && request.dto.PageNumber > 0)
        //        {
        //            _spec.IsPagination = true;
        //            _spec.Take = request.dto.PageSize;
        //            _spec.Skip = (request.dto.PageNumber - 1) * request.dto.PageSize;
        //        }

        //        var trips =  await _repo.GetAllSpec(_spec).Select(x=>new TemplateTrip
        //        {
        //            Id = x.Id,
        //            Title = x.Title,
        //            Destination = x.Destination,
        //            Duration = x.Duration,
        //            StartDate = x.StartDate,
        //            From = x.From,
        //            IncludedPackages = Enum.GetValues(typeof(Package))
        //.Cast<Package>()
        //.Where(p => p != Package.None && x.IncludedPackages.HasFlag(p))
        //.Select(p => (int)p)
        //.ToList(),
        //            NumberOfMember = x.CurrentNumberOfMember,
        //            TripCategory = x.TripCategory,
        //            Price = x.Price,
        //            Activities = x.PublicActivities.Select(x=>new TemplateActivity
        //            {
        //                Id = x.Id,
        //                Title = x.Title,
        //                DataId = x.DataId,
        //                Description = x.Description,
        //                Destination = x.Destination,
        //                ActivityType = x.ActivityType,
        //                SelectedDay = x.SelectedDay,
        //                Address = x.Address,
        //                EndAt = x.EndAt,
        //                FullPrice = x.FullPrice,
        //                Image = x.Image,
        //                Latitude = x.Latitude,
        //                Longitude = x.Longitude,
        //                Phone = x.Phone,
        //                PlaceId = x.PlaceId,
        //                PriceRange = x.PriceRange,
        //                Rating = x.Rating,
        //                Reviews = x.Reviews,
        //                StartAt = x.StartAt,
        //                TripCategory = x.TripCategory,
        //                Website = x.Website,
        //                serviceOption = !string.IsNullOrEmpty(x.serviceOption)
        //                ? x.serviceOption.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
        //                : new List<string>()
        //            }).ToList()
        //        }).ToListAsync();
        //        if (trips.Any())
        //            return new ApiResultResponse<List<TemplateTrip>>(200, trips, "Trips retrieved successfully");
        //        return new ApiResponse(404, "not found");
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponse(500,ex.Message);
        //    }
        //}

        //public async Task<ApiResponse> Handle(GetAllTrip request, CancellationToken cancellationToken)
        //{
        //    var trips = await _repo.GetAll().Select(x => new TemplateTrip
        //    {
        //        Id = x.Id,
        //        Title = x.Title,
        //        Destination = x.Destination,
        //        Duration = x.Duration,
        //        StartDate = x.StartDate,
        //        From = x.From,
        //        IncludedPackages = Enum.GetValues(typeof(Package)).Cast<Package>().Where(p => p != Package.None && x.IncludedPackages.HasFlag(p)).Select(p => (int)p).ToList(),
        //        NumberOfMember = x.CurrentNumberOfMember,
        //        TripCategory = x.TripCategory,
        //        Price = x.Price,
        //        Activities = x.PublicActivities.Select(x => new TemplateActivity
        //        {
        //            Id = x.Id,
        //            Title = x.Title,
        //            DataId = x.DataId,
        //            Description = x.Description,
        //            Destination = x.Destination,
        //            ActivityType = x.ActivityType,
        //            SelectedDay = x.SelectedDay,
        //            Address = x.Address,
        //            EndAt = x.EndAt,
        //            FullPrice = x.FullPrice,
        //            Image = x.Image,
        //            Latitude = x.Latitude,
        //            Longitude = x.Longitude,
        //            Phone = x.Phone,
        //            PlaceId = x.PlaceId,
        //            PriceRange = x.PriceRange,
        //            Rating = x.Rating,
        //            Reviews = x.Reviews,
        //            StartAt = x.StartAt,
        //            TripCategory = x.TripCategory,
        //            Website = x.Website,
        //            serviceOption = !string.IsNullOrEmpty(x.serviceOption)
        //                ? x.serviceOption.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
        //                : new List<string>()
        //        }).ToList()
        //    }).ToListAsync();
        //    if (trips.Any())
        //        return new ApiResultResponse<List<TemplateTrip>>(200, trips, "Trips retrieved successfully");
        //    return new ApiResponse(404, "not found");
        //}


        public async Task<ApiResponse> Handle(GetPubTripSpecQuery request, CancellationToken cancellationToken)
        {
            var spec = new PublicTripSpecification(request.req);

            // 1. جلب البيانات من القاعدة أولاً (بدون Select معقد) 
            // تأكد أن GetAllSpec تعيد IQueryable
            var tripsFromDb = await _repo.GetAllSpec(spec).ToListAsync(cancellationToken);

            if (tripsFromDb == null || !tripsFromDb.Any())
            {
                return new ApiResponse(404, "No trips found");
            }

            // 2. القيام بعملية الـ Mapping في الذاكرة (In-Memory Mapping)
            // هنا يمكنك استخدام Split وأي كود C# براحتك لأن البيانات أصبحت في الذاكرة
            var result = tripsFromDb.Select(x => new TemplateTrip
            {
                Id = x.Id,
                Title = x.Title,
                Destination = x.Destination,
                Duration = x.Duration,
                StartDate = x.StartDate ?? DateTime.MinValue,
                From = x.From,
                IncludedPackages = Enum.GetValues(typeof(Package))
                    .Cast<Package>()
                    .Where(p => p != Package.None && x.IncludedPackages.HasFlag(p))
                    .Select(p => (int)p)
                    .ToList(),
                NumberOfMember = x.CurrentNumberOfMember,
                TripCategory = x.TripCategory,
                Price = x.Price,
                TravelerFee = x.TravelerFee, 
                Activities = x.PublicActivities?.Select(a => new TemplateActivity
                {
                    Id = a.Id,
                    Title = a.Title,
                    FullPrice = a.FullPrice,
                    Destination = a.Destination,
                    EndAt = a.EndAt,
                    Image = a.Thumbnail ?? a.Image,
                    SelectedDay = a.SelectedDay,
                    StartAt = a.StartAt,
                    TripCategory = a.TripCategory,
                    PlaceId = a.PlaceId,
                    DataId = a.DataId,
                    ActivityType = a.ActivityType,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    Address = a.Address,
                    Website = a.Website,
                    Phone = a.Phone,
                    Rating = a.Rating,
                    Reviews = a.Reviews,
                    PriceRange = a.PriceRange,
                    Description = a.Description,
                    // الـ Split سيعمل هنا بنجاح لأننا في Memory
                    serviceOption = !string.IsNullOrEmpty(a.serviceOption)
                        ? a.serviceOption.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
                        : new List<string>()
                }).ToList() ?? new List<TemplateActivity>()
            }).ToList();

            // 3. الإرجاع
            if (request.req.Id.HasValue)
            {
                return new ApiResultResponse<TemplateTrip>(200, result.First());
            }

            return new ApiResultResponse<List<TemplateTrip>>(200, result);
        }


    }
    public class PrivateTripQueryHandler : 
        IQueryHandler<GetPrivateTripforUserId, ApiResponse>,
        IQueryHandler<GetPrivTripSpecQuery, ApiResponse>
    {
        private IReadGenericRepo<PrivateTrip> _repo;

        private ISpecification<PrivateTrip> _spec;
        public PrivateTripQueryHandler(IReadGenericRepo<PrivateTrip> repo, ISpecification<PrivateTrip> spec)
        {
            _repo = repo;
            _spec = spec;
        }

        public async Task<ApiResponse> Handle(GetPrivateTripforUserId request, CancellationToken cancellationToken)
        {

            var trips =await _repo.GetAll().Where(x=>x.CreatedById == request.id).Select(
                TripMappingExtensions.MapToPrivateTemplate
            ).ToListAsync();

            if (trips.Any())
                return new ApiResultResponse<List<PrivateTemplateTrip>>(200, trips, "Trips retrieved successfully");
            return new ApiResponse(404, "not found");
        }

        public async Task<ApiResponse> Handle(GetPrivTripSpecQuery request, CancellationToken cancellationToken)
        {
            // 1. تعريف الـ Specification
            var spec = new PrivateTripSpecification(request.req);

            // 2. جلب البيانات من الداتا بيز (In-Memory) لتجنب مشاكل الترجمة (Translation Error)
            var items = await _repo
                .GetAllSpec(spec)
                .ToListAsync(cancellationToken);

            // 3. التحقق من وجود بيانات
            if (items == null || !items.Any())
            {
                return new ApiResponse(404, "No private trips found");
            }

            // 4. معالجة البيانات وإرجاع الـ Response
            if (request.req.Id.HasValue)
            {
                var item = items.FirstOrDefault(); // استخدام FirstOrDefault أضمن
                if (item == null) return new ApiResponse(404);

                var singleResult = MapToPrivateTemplateTrip(item);
                return new ApiResultResponse<PrivateTemplateTrip>(200, singleResult);
            }

            // في حالة البحث العام بنرجع List
            var resultList = items.Select(x => MapToPrivateTemplateTrip(x)).ToList();
            return new ApiResultResponse<List<PrivateTemplateTrip>>(200, resultList);
        }

        // دالة الـ Mapping المصلحة
         private PrivateTemplateTrip MapToPrivateTemplateTrip(PrivateTrip x)
        {
            return new PrivateTemplateTrip
            {
                CreatedById = x.CreatedById,
                Id = x.Id,
                Title = x.Title,
                Destination = x.Destination,
                Duration = x.Duration,
                StartDate = x.StartDate,
                From = x.From,
                TripCategory = x.TripCategory,
                Price = x.Price,

                Activities = x.PrivateActivities?.Select(a => new TemplateActivity
                {
                    Id = a.Id,
                    TripCategory = a.TripCategory,
                    Destination = a.Destination,
                    EndAt = a.EndAt,
                    FullPrice = a.FullPrice,
                    Image = a.Image,
                    SelectedDay = a.SelectedDay,
                    StartAt = a.StartAt,
                    Title = a.Title,
                    PlaceId = a.PlaceId,
                    DataId = a.DataId,
                    ActivityType = a.ActivityType,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    Address = a.Address,
                    Website = a.Website,
                    Phone = a.Phone,
                    Rating = a.Rating,
                    Reviews = a.Reviews,
                    PriceRange = a.PriceRange,
                    Description = a.Description,
                    // تصحيح مشكلة الـ Split والـ Null
                    serviceOption = !string.IsNullOrEmpty(a.serviceOption)
                        ? a.serviceOption.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
                        : new List<string>()
                }).ToList() ?? new List<TemplateActivity>()
            };
        }
        
    
    }


   
}
