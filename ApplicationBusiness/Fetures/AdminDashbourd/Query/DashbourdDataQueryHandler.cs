using Application.Abstraction.message;
using Domain.Abstraction;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.AdminDashbourd.Query
{
    public record DashbourdDataQuery() : IQuery<ApiResponse>;
    internal class DashbourdDataQueryHandler : IQueryHandler<DashbourdDataQuery, ApiResponse>
    {
        private IDashDashbourdDataQuery dashDashbourdDataQuery;

        public DashbourdDataQueryHandler(IDashDashbourdDataQuery dashDashbourdDataQuery)
        {
            this.dashDashbourdDataQuery = dashDashbourdDataQuery;
        }

        public async Task<ApiResponse> Handle(DashbourdDataQuery request, CancellationToken cancellationToken)
        {
            var res = await dashDashbourdDataQuery.GetDashbourdDataDtoAsync();
            return new ApiResultResponse<GetDashbourdDataDto>(200, res
                );
        }
    }
}
