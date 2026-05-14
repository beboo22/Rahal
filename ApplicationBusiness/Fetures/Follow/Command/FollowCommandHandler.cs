using Application.Abstraction.message;
using Application.Fetures.Authentication.Command.Models;
using ApplicationBusiness.Fetures.Follow.Command.Models;
using ApplicationBusiness.Fetures.Follow.Query.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Follow.Command
{
    internal class FollowCommandHandler : 
        ICommandHandler<FollowComand, ApiResponse>,
        ICommandHandler<UnfollowCommand, ApiResponse>
    {
        private IWriteGenericRepo<UserFollow> _userWriteGenericRepo;
        IWriteUnitOfWork _unitOfWork;
        public ISender Sender { get; set; }

        public FollowCommandHandler(IWriteGenericRepo<UserFollow> userWriteGenericRepo, IWriteUnitOfWork unitOfWork, ISender sender)
        {
            _userWriteGenericRepo = userWriteGenericRepo;
            _unitOfWork = unitOfWork;
            Sender = sender;
        }

        public async Task<ApiResponse> Handle(FollowComand request, CancellationToken cancellationToken)
        {
            // 1. منع المتابعة الذاتية
            if (request.person == request.Follower)
            {
                return new ApiResponse(400, "User cannot follow themselves");
            }
            var checkUserExitance = await Sender.Send(new IsUserExist(request.person));
            if (checkUserExitance.statusCode != 200)
            {
                return new ApiResponse(
                    404,
                    "person not fount"
                );
            }
            checkUserExitance = await Sender.Send(new IsUserExist(request.Follower));
            if (checkUserExitance.statusCode != 200)
            {
                return new ApiResponse(   
                    404,
                    "follower not fount"
                );
            }
            var isAlreadyFollowing = await Sender.Send(new IsFollowingQuery(request.Follower, request.person));
            if (isAlreadyFollowing.statusCode != 404)
                return new ApiResponse(409, "Already following");
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _userWriteGenericRepo.AddAsync(new UserFollow { FollowerId = request.Follower, FollowingId = request.person });
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new ApiResponse(500, ex.InnerException?.Message);
            }
        }

        public async Task<ApiResponse> Handle(UnfollowCommand request, CancellationToken cancellationToken)
        {
            // 1. منع إلغاء المتابعة الذاتية (اختياري)
            if (request.Follower == request.person)
            {
                return new ApiResponse(400, "User cannot unfollow themselves");
            }

            // 2. التحقق من وجود اليوزرز (Queries)
            var personExists = await Sender.Send(new IsUserExist(request.person), cancellationToken);
            if (personExists.statusCode != 200)
            {
                return new ApiResponse(404, "Person not found");
            }

            var followerExists = await Sender.Send(new IsUserExist(request.Follower), cancellationToken);
            if (followerExists.statusCode != 200)
            {
                return new ApiResponse(404, "Follower not found");
            }

            // 3. ✅ التحقق من وجود علاقة متابعة قبل الحذف
            
            var followRelation = await Sender.Send(new IsFollowingQuery(request.Follower, request.person));
            if (followRelation.statusCode == 404)
                return new ApiResponse(409, "Already following");
            // 4. Unfollow logic (Delete)
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var item = followRelation as ApiResultResponse<UserFollow>;


                // حذف العلاقة
                await _userWriteGenericRepo.DeleteAsync(item.Data.Id);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return new ApiResponse(200, "Unfollowed successfully");
            }
            catch (DbUpdateConcurrencyException ex) // ✅ معالجة مشاكل التوافقية
            {
                await _unitOfWork.RollbackAsync();
                return new ApiResponse(409, "Concurrency conflict - please try again");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new ApiResponse(500, ex.InnerException?.Message ?? "Internal server error");
            }
        }
    }
}
