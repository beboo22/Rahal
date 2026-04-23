using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.SerpApiService;
using ApplicationBusiness.Fetures.PhotoService.Query.Model;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PhotoService.Query
{
    internal class GetPhotoQueryHandler : IQueryHandler<GetPhotoQuery, ApiResponse>
    {
        private readonly ISerpPhotoApiService _serpPhotoApiService;

        public GetPhotoQueryHandler(ISerpPhotoApiService serpPhotoApiService)
        {
            _serpPhotoApiService = serpPhotoApiService;
        }

        public async Task<ApiResponse> Handle(GetPhotoQuery request, CancellationToken cancellationToken)
        {
            return await _serpPhotoApiService.SearchPhotoAsync(request.Req, cancellationToken);
        }
    }
}
