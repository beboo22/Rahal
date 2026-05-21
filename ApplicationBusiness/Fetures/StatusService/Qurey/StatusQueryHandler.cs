using Application.Abstraction.message;
using ApplicationBusiness.Fetures.StatusService.Qurey.res;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Status;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.StatusService.Qurey
{
    public record getStatusForUser(int UserId):IQuery<ApiResponse>;

    internal class StatusQueryHandler : IQueryHandler<getStatusForUser, ApiResponse>
    {
        private IReadGenericRepo<Status> Repo;

        public StatusQueryHandler(IReadGenericRepo<Status> repo)
        {
            Repo = repo;
        }

        public async Task<ApiResponse> Handle(getStatusForUser request, CancellationToken cancellationToken)
        {
            var item  = await Repo.GetAll().Where(x => x.CreatedById == request.UserId).Select(x=>new TemplateStatus
            {

                Id = x.Id,
                ItemUrl = x.ItemUrl,
                Title = x.Title,
            }).ToListAsync();
            if (item == null)
                return new ApiResponse(404);

            return new ApiResultResponse<List<TemplateStatus>>(200, item);


        }
    }
}
