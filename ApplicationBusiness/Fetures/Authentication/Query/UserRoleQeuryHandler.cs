using Application.Abstraction.message;
using Application.Fetures.Authentication.Query.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Authentication.Query
{
    internal class UserRoleQeuryHandler : IQueryHandler<GetRoleofUser, ApiResponse>
    {
        private IReadGenericRepo<UserRole> _rRepo;

        public UserRoleQeuryHandler(IReadGenericRepo<UserRole> rRepo)
        {
            _rRepo = rRepo;
        }

        public async Task<ApiResponse> Handle(GetRoleofUser request, CancellationToken cancellationToken)
        {
            var roles = await _rRepo.GetAll().Include(x => x.Role).Where(x => x.UserId == request.UserId).Select(x => x.Role.RoleName).ToListAsync();
             return   new ApiResultResponse<List<RoleEnum>>(200,roles);
        }
    }
}
