using Application.Abstraction.message;
using Application.Abstraction.Specification;
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
                    IncludedPackages = x.IncludedPackages,
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
                IncludedPackages = x.IncludedPackages,
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
    }
    public class PrivateTripQueryHandler : IQueryHandler<GetPrivateTripforUserId, ApiResponse>
    {
        private IReadGenericRepo<PrivateTrip> _repo;

        public PrivateTripQueryHandler(IReadGenericRepo<PrivateTrip> repo)
        {
            _repo = repo;
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
                StartDate = x.StartDate,
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
