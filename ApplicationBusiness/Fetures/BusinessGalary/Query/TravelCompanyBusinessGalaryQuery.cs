using Application.Abstraction.message;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BusinessGalary.Query
{
    public record GetTravelCompanyBusinessGalary(int travelCompanyId) : IQuery<ApiResponse>;
    internal class TravelCompanyBusinessGalaryQuery : IQueryHandler<GetTravelCompanyBusinessGalary, ApiResponse>
    {
        private IReadGenericRepo<TravelCompanyBusinessGalary> readGeneric;

        public TravelCompanyBusinessGalaryQuery(IReadGenericRepo<TravelCompanyBusinessGalary> readGeneric)
        {
            this.readGeneric = readGeneric;
        }

        public async Task<ApiResponse> Handle(GetTravelCompanyBusinessGalary request, CancellationToken cancellationToken)
        {
            var galary = await readGeneric.GetAll().Where(x => x.TravelCompanyId == request.travelCompanyId).ToListAsync();
            if (!galary.Any())
                return new ApiResponse(404, "No Galary Found For This Travel Company");
            return new ApiResultResponse<List<TravelCompanyBusinessGalary>>(200, galary);

        }
    }
}
