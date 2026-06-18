using Application.Abstraction.message;
using Application.Fetures.Authentication.Command.Models;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Fetures.Authentication.Command.Models;
using ApplicationBusiness.Fetures.NotficationSystem.Command.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Authentication.Command
{
    public record verfiedTourguideAndTravelCompany(int tourguideId) : ICommand<ApiResponse>;
    public record BlockUser(int userId,DateTime BlockedStartDate,DateTime BlockedEndDate) : ICommand<ApiResponse>;
    internal class UserCommandHandler : ICommandHandler<UpdateUsers, ApiResponse>,
                                                ICommandHandler<IsUserExist, ApiResponse>,
                                                ICommandHandler<verfiedTourguideAndTravelCompany, ApiResponse>,
                                                ICommandHandler<BlockUser, ApiResponse>

    {
        IWriteUnitOfWork UnitOfWork;
        IWriteUserRepo Repo;
        IReadGenericRepo<User> RRepo;
        public ISender Sender { get; set; }

        public UserCommandHandler(IWriteUnitOfWork unitOfWork, IWriteUserRepo repo, IReadGenericRepo<User> rRepo, ISender sender)
        {
            UnitOfWork = unitOfWork;
            Repo = repo;
            RRepo = rRepo;
            Sender = sender;
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

        public async Task<ApiResponse> Handle(verfiedTourguideAndTravelCompany request, CancellationToken cancellationToken)
        {
            try
            {
                await UnitOfWork.BeginTransactionAsync();
                var user = await RRepo.GetByIdAsync(request.tourguideId);
                if (user == null)
                    return new ApiResponse(404, "User not found.");
                user.Isverified = true;
                await Repo.UpdateAsync(user,user.Id);
                await UnitOfWork.SaveChangesAsync();
                await UnitOfWork.CommitAsync();

                await Sender.Send(
                        new SendSystemNotificationCommand(
                            request.tourguideId.ToString(),
                            "Ur Profile Has Verified",
                            $"{user.FName} {user.LName}.",
                            ""
                        ));

                return new ApiResponse(200, "User verified successfully.");
            }
            catch (Exception ex)
            {
                await UnitOfWork.RollbackAsync();
                return new ApiResponse(500, $"Error while verifying user: {ex.Message}");
            }

        }

        public async Task<ApiResponse> Handle(BlockUser request, CancellationToken cancellationToken)
        {

            try
            {
                await UnitOfWork.BeginTransactionAsync();
                var user = await RRepo.GetByIdAsync(request.userId);
                if (user == null)
                    return new ApiResponse(404, "User not found.");
                user.IsBlocked = true;
                user.BlockedStartDate = request.BlockedStartDate;
                user.BlockedEndDate = request.BlockedEndDate;
                user.BlockedCounter++;
                if(user.BlockedCounter >= 3)
                {
                    user.IsBlocked = true;
                    user.BlockedStartDate = DateTime.UtcNow;
                    user.BlockedEndDate = DateTime.UtcNow.AddYears(1);
                }

                await Repo.UpdateAsync(user, user.Id);
                await UnitOfWork.SaveChangesAsync();
                await UnitOfWork.CommitAsync();

                await Sender.Send(
                        new SendSystemNotificationCommand(
                            request.userId.ToString(),
                            "Ur Profile Has Blocked",
                            $"Admin Blocked U to {user.BlockedEndDate}.",
                            ""
                        ));

                return new ApiResponse(200, "User verified successfully.");
            }
            catch (Exception ex)
            {
                await UnitOfWork.RollbackAsync();
                return new ApiResponse(500, $"Error while verifying user: {ex.Message}");
            }
        }
    }
}
