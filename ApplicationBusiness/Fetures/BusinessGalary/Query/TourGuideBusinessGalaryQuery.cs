using Application.Abstraction.message;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.TourGuidEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BusinessGalary.Query
{
    public record GetTourguideBusinessGalary(int tourguide):IQuery<ApiResponse>;
    internal class TourGuideBusinessGalaryQuery: IQueryHandler<GetTourguideBusinessGalary, ApiResponse>
    {
        private IReadGenericRepo<TourGuideBusinessGalary> readGeneric;

        public TourGuideBusinessGalaryQuery(IReadGenericRepo<TourGuideBusinessGalary> readGeneric)
        {
            this.readGeneric = readGeneric;
        }

        public async Task<ApiResponse> Handle(GetTourguideBusinessGalary request, CancellationToken cancellationToken)
        {
            var galary = await readGeneric.GetAll().Where(x => x.TourGuidId == request.tourguide).ToListAsync();
            if (!galary.Any())
                return new ApiResponse(404, "No Galary Found For This Tour Guide");
            return new ApiResultResponse<List<TourGuideBusinessGalary>>(200, galary);
        }
    }
}
