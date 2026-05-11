using Application.Abstraction.message;
using Application.Fetures.Authentication.Command.Models;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Fetures.Authentication.Command.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Authentication.Command
{
    internal class UserCommandHandler : ICommandHandler<UpdateUsers, ApiResponse>,
                                                ICommandHandler<IsUserExist, ApiResponse>

    {
        IWriteUnitOfWork UnitOfWork;
        IWriteUserRepo Repo;
        public UserCommandHandler(IWriteUnitOfWork unitOfWork, IWriteUserRepo repo)
        {
            UnitOfWork = unitOfWork;
            Repo = repo;
        }
        public async Task<ApiResponse> Handle(IsUserExist request, CancellationToken cancellationToken)
        {
            var exists = await Repo.ExistsAsync(request.UserId);
            if (exists)
                return new ApiResponse((int)HttpStatusCode.OK, "User exists.");
            else
                return new ApiResponse((int)HttpStatusCode.NotFound, "User does not exist.");
        }
        public async Task<ApiResponse> Handle(UpdateUsers request, CancellationToken cancellationToken)
        {
            try
            {
                await UnitOfWork.BeginTransactionAsync();
                await Repo.UpdateRangeAsync(request.Users);
                await UnitOfWork.SaveChangesAsync();
                await UnitOfWork.CommitAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                await UnitOfWork.RollbackAsync();
                return new ApiResponse(500,$"error while update User {ex.InnerException}");
            }
        }
    }
}
