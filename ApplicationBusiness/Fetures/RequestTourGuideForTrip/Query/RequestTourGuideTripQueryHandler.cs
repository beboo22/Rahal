using Application.Abstraction.message;
using ApplicationBusiness.Fetures.RequestTourGuideForTrip.Query.Response;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.TripEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.RequestTourGuideForTrip.Query
{
    public record GetRequsetForTougiude(int TourguideId) : IQuery<ApiResponse>;
    internal class RequestTourGuideTripQueryHandler : IQueryHandler<GetRequsetForTougiude, ApiResponse>
    {
        private readonly IReadGenericRepo<RequestTourGuidePrivateTrip> _privRepo;
        private readonly IReadGenericRepo<RequestTourGuidePulicTrip> _pubRepo;

        public RequestTourGuideTripQueryHandler(
            IReadGenericRepo<RequestTourGuidePrivateTrip> privRepo,
            IReadGenericRepo<RequestTourGuidePulicTrip> pubRepo)
        {
            _privRepo = privRepo;
            _pubRepo = pubRepo;
        }

        public async Task<ApiResponse> Handle(GetRequsetForTougiude request, CancellationToken cancellationToken)
        {
            // 1. Fetch private requests directly projected with their RequestId
            var privateRequests = await _privRepo.GetAll()
                .Where(x => x.TourGuideId == request.TourguideId && x.Accept == false)
                .Select(RequestMappingExtensions.MapToPrivateRequestDto)
                .ToListAsync(cancellationToken);

            // 2. Fetch public requests directly projected with their RequestId
            var publicRequests = await _pubRepo.GetAll()
                .Where(x => x.TourGuideId == request.TourguideId && x.Accept == false)
                .Select(RequestMappingExtensions.MapToPublicRequestDto)
                .ToListAsync(cancellationToken);

            var res = new TemplateRequestTourGuide
            {
                PrivateRequests = privateRequests,
                PublicRequests = publicRequests
            };

            return new ApiResultResponse<TemplateRequestTourGuide>(200, res);
        }
    }
}
