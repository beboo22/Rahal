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
            var item  = await Repo.GetAll().FirstOrDefaultAsync(x => x.CreatedById == request.UserId);
            if (item == null)
                return new ApiResponse(404);

            return new ApiResultResponse<TemplateStatus>(200, new TemplateStatus
            {
                Id = item.Id,
                ItemUrl = item.ItemUrl,
                Title = item.Title,
            });


        }
    }
}
