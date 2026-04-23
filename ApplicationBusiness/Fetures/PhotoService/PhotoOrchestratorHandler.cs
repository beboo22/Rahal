using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.SerpApiService;
using ApplicationBusiness.Fetures.PhotoService.Command.Model;
using ApplicationBusiness.Fetures.PhotoService.Query.Model;
using Domain.BaseResponce;
using Domain.Entity.photo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PhotoService
{
    internal class PhotoOrchestratorHandler : IQueryHandler<PhotoSearchOrchestratorQuery, ApiResponse>
    {
        private readonly IMediator _mediator;

        public PhotoOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ApiResponse> Handle(PhotoSearchOrchestratorQuery request, CancellationToken cancellationToken)
        {
            // 1. Call SerpAPI via GetPhotoQuery
            var photoResult = await _mediator.Send(new GetPhotoQuery(request.Request), cancellationToken);

            if (photoResult is not ApiResultResponse<PhotoSearchResponse> response || response.Data is null)
            {
                return photoResult;
            }

            // If status is 224 (Exact Cache Hit from Service level), return immediately
            if (response.statusCode == 224)
                return response;

            // 2. Prepare Cache Keys
            var exactKey = CacheKeys.PhotoExact(request.Request);
            var exactKeyOrgin = CacheKeys.PhotoExactOrgin(request.Request);
            // Using same key for group if images don't have complex grouping like hotels
            var groupKey = CacheKeys.PhotoGroup(request.Request);

            // 3. Persist to DB and Cache via Command
            var saveResult = await _mediator.Send(
                new SavePhotoCommand(response.Data, exactKey,exactKeyOrgin, groupKey),
                cancellationToken);

            return saveResult;
        }
    }
}
