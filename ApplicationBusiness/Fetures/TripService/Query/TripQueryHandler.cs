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
    public class PublicTripQueryHandler : IQueryHandler<GetAllTrip, ApiResponse>,
                                         IQueryHandler<GetPubTripSpecQuery, ApiResponse>,
                                        IQueryHandler<SearchForTrip, ApiResponse>
    {
        private IReadGenericRepo<PublicTrip> _repo;
        private ISpecification<PublicTrip> _spec;
        public PublicTripQueryHandler(IReadGenericRepo<PublicTrip> repo, ISpecification<PublicTrip> spec)
        {
            _repo = repo;
            _spec = spec;
        }

        public async Task<ApiResponse> Handle(SearchForTrip request, CancellationToken cancellationToken)
        {
            try
            {
                if (request?.dto == null)
                {
                    return new ApiResponse
                    (
                        400,
                        "Invalid request data"
                    );
                }

                Expression<Func<PublicTrip, bool>>? criteria = null;

                if (!string.IsNullOrEmpty(request.dto.Title))
                {
                    criteria = trip => trip.Title.Contains(request.dto.Title);
                }

                if (!string.IsNullOrEmpty(request.dto.Destination))
                {
                    // Combine with previous condition if it exists
                    if (criteria != null)
                        criteria = criteria.AndAlso(trip => trip.Destination.Contains(request.dto.Destination));
                    else
                        criteria = trip => trip.Destination.Contains(request.dto.Destination);
                }

                _spec.crateria = criteria;

                // Example: Add pagination if provided in request.dto
                if (request.dto.PageSize > 0 && request.dto.PageNumber > 0)
                {
                    _spec.IsPagination = true;
                    _spec.Take = request.dto.PageSize;
                    _spec.Skip = (request.dto.PageNumber - 1) * request.dto.PageSize;
                }

                var trips =  await _repo.GetAllSpec(_spec).Select(x=>new TemplateTrip
                {
                    Id = x.Id,
                    Title = x.Title,
                    Destination = x.Destination,
                    Duration = x.Duration,
                    StartDate = x.StartDate,
                    From = x.From,
                    IncludedPackages = Enum.GetValues(typeof(Package))
        .Cast<Package>()
        .Where(p => p != Package.None && x.IncludedPackages.HasFlag(p))
        .Select(p => (int)p)
        .ToList(),
                    NumberOfMember = x.CurrentNumberOfMember,
                    TripCategory = x.TripCategory,
                    Price = x.Price,
                    Activities = x.PublicActivities.Select(x=>new TemplateActivity
                    {
                        Id = x.Id,
                        TripCategory = x.TripCategory,
                        Destination = x.Destination,
                        EndAt = x.EndAt,
                        FullPrice = x.FullPrice,
                        Image = x.Image,
                        SelectedDay = x.SelectedDay,
                        StartAt = x.StartAt,
                        Title = x.Title
                    }).ToList()
                }).ToListAsync();
                if (trips.Any())
                    return new ApiResultResponse<List<TemplateTrip>>(200, trips, "Trips retrieved successfully");
                return new ApiResponse(404, "not found");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500,ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(GetAllTrip request, CancellationToken cancellationToken)
        {
            var trips = await _repo.GetAll().Select(x => new TemplateTrip
            {
                Id = x.Id,
                Title = x.Title,
                Destination = x.Destination,
                Duration = x.Duration,
                StartDate = x.StartDate,
                From = x.From,
                IncludedPackages = Enum.GetValues(typeof(Package)).Cast<Package>().Where(p => p != Package.None && x.IncludedPackages.HasFlag(p)).Select(p => (int)p).ToList(),
                NumberOfMember = x.CurrentNumberOfMember,
                TripCategory = x.TripCategory,
                Price = x.Price,
                Activities = x.PublicActivities.Select(x => new TemplateActivity
                {
                    Id = x.Id,
                    TripCategory = x.TripCategory,
                    Destination = x.Destination,
                    EndAt = x.EndAt,
                    FullPrice = x.FullPrice,
                    Image = x.Image,
                    SelectedDay = x.SelectedDay,
                    StartAt = x.StartAt,
                    Title = x.Title
                }).ToList()
            }).ToListAsync();
            if (trips.Any())
                return new ApiResultResponse<List<TemplateTrip>>(200, trips, "Trips retrieved successfully");
            return new ApiResponse(404, "not found");
        }

        public async Task<ApiResponse> Handle(GetPubTripSpecQuery request, CancellationToken cancellationToken)
        {
            // Fetch data using the spec
            var spec = new PublicTripSpecification(request.req);


            var items = await _repo.GetAllSpec(spec).ToListAsync();

            if (items == null || !items.Any())
                return new ApiResponse(404);

            // Map to DTO (TemplateTrip)
            var result = items.Select(x => new TemplateTrip
            {
                Id = x.Id,
                Title = x.Title,
                Destination = x.Destination,
                Duration = x.Duration,
                StartDate = x.StartDate ?? DateTime.MinValue, // Handle nullable
                From = x.From,
                IncludedPackages = Enum.GetValues(typeof(Package)).Cast<Package>().Where(p => p != Package.None && x.IncludedPackages.HasFlag(p)).Select(p => (int)p).ToList(),
                NumberOfMember = x.CurrentNumberOfMember,
                TripCategory = x.TripCategory,
                Price = x.Price,
                Activities = x.PublicActivities?.Select(a => new TemplateActivity
                {
                    Id = a.Id,
                    Title = a.Title,
                    FullPrice = a.FullPrice,
                    Destination = a.Destination,
                    EndAt = a.EndAt,
                    Image = a.Image,
                    SelectedDay = a.SelectedDay,
                    StartAt = a.StartAt,
                    TripCategory= a.TripCategory,
                    // ... rest of your mapping
                }).ToList() ?? new List<TemplateActivity>()
            }).ToList();

            // If ID was requested, you might want to return the single object instead of a list
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

            var trips =await _repo.GetAll().Where(x=>x.CreatedById == request.id).Select(x => new PrivateTemplateTrip
            {
                Id = x.Id,
                Title = x.Title,
                Destination = x.Destination,
                Duration = x.Duration,
                From = x.From,
                TripCategory = x.TripCategory,
                Price = x.Price,
                CustomizationFee = x.CustomizationFee,
                //Reviews = x.Reviews,
                TourGuideId = x.TourGuideId,
                StartDate =  x.StartDate.Value,
                Activities = x.PrivateActivities.Select(x => new TemplateActivity
                {
                    Id = x.Id,
                    TripCategory = x.TripCategory,
                    Destination = x.Destination,
                    EndAt = x.EndAt,
                    FullPrice = x.FullPrice,
                    Image = x.Image,
                    SelectedDay = x.SelectedDay,
                    StartAt = x.StartAt,
                    Title = x.Title
                }).ToList()
            }).ToListAsync();

            if (trips.Any())
                return new ApiResultResponse<List<PrivateTemplateTrip>>(200, trips, "Trips retrieved successfully");
            return new ApiResponse(404, "not found");
        }

        public async Task<ApiResponse> Handle(GetPrivTripSpecQuery request, CancellationToken cancellationToken)
        {
            // 1. تعريف الـ Specification مرة واحدة عشان نستخدمها
            var spec = new PrivateTripSpecification(request.req);

            // 2. جلب البيانات من الداتا بيز
            // بنستخدم ToListAsync مباشرة لأن الـ Spec جواه الـ Pagination والـ Includes
            var items = await _repo
                .GetAllSpec(spec)
                .ToListAsync(cancellationToken);

            // 3. التحقق من وجود بيانات
            if (items == null || !items.Any())
            {
                return new ApiResponse(404);
            }

            // 4. لو المستخدم باعت Id محدد، نرجع عنصر واحد بس في الـ Response
            if (request.req.Id.HasValue)
            {
                var item = items.First(); // لأنه كده كده هيرجع عنصر واحد بسبب الـ Id

                // تحويل العنصر لـ DTO (PrivateTemplateTrip)
                var singleResult = MapToPrivateTemplateTrip(item);
                return new ApiResultResponse<PrivateTemplateTrip>(200, singleResult);
            }

            // 5. في حالة البحث العام (Search Flow) بنرجع List
            var resultList = items.Select(x => MapToPrivateTemplateTrip(x)).ToList();

            return new ApiResultResponse<List<PrivateTemplateTrip>>(200, resultList);
        }

        // دالة مساعدة (Helper Method) للـ Mapping عشان الكود يبقى نضيف وميتكررش
        private PrivateTemplateTrip MapToPrivateTemplateTrip(PrivateTrip x)
        {
            return new PrivateTemplateTrip
            {
                Id = x.Id,
                Title = x.Title,
                Destination = x.Destination,
                Duration = x.Duration,
                StartDate = x.StartDate,
                From = x.From,
                TripCategory = x.TripCategory,
                Price = x.Price,
                // تأكد إن PrivateActivities مش null قبل الـ Select
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
                    Title = a.Title
                }).ToList() ?? new List<TemplateActivity>()
            };
        }
    }


    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var combined = Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    Expression.Invoke(expr1, parameter),
                    Expression.Invoke(expr2, parameter)
                ),
                parameter
            );

            return combined;
        }
    }

}
