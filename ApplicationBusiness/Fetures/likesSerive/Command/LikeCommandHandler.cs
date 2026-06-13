using Application.Abstraction.message;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Fetures.Authentication.Query;
using ApplicationBusiness.Fetures.likesSerive.Command.Models;
using ApplicationBusiness.Fetures.NotficationSystem.Command.Models;
using ApplicationBusiness.Fetures.PostService.Query.Models;
using ApplicationBusiness.Fetures.PostService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.PostEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.likesSerive.Command
{
    internal class LikeCommandHandler : ICommandHandler<AddLike, ApiResponse>
    {
        private IWriteGenericRepo<Likes> _rWepo;
        private IReadGenericRepo<Likes> _rRepo;
        private IWriteUnitOfWork _unitOfWork;
        public ISender Sender { get; set; }

        public LikeCommandHandler(IWriteGenericRepo<Likes> repo, IWriteUnitOfWork unitOfWork, IReadGenericRepo<Likes> rRepo, ISender sender)
        {
            _rWepo = repo;
            _unitOfWork = unitOfWork;
            _rRepo = rRepo;
            Sender = sender;
        }
        private static string GetLikeReactionText(LikeType likeType)
        {
            return likeType switch
            {
                LikeType.love => "reacted ❤️ to your post",
                LikeType.hahaha => "reacted 😂 to your post",
                LikeType.sad => "reacted 😢 to your post",
                LikeType.angry => "reacted 😠 to your post",
                _ => "reacted to your post"
            };
        }
        public async Task<ApiResponse> Handle(AddLike request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Check if the like already exists (assuming a composite key or specific criteria)
                // Adjust the predicate (l => ...) based on your actual Likes entity properties
                var existingLike = await _rRepo.GetAll().Where(l => l.UserId == request.UserId && l.postId == request.postId).FirstOrDefaultAsync();
                await _unitOfWork.BeginTransactionAsync();
                var checkPostExitance = await Sender.Send(new GetExperienceSpacificationPost(null, request.postId, null,null,null,true, null,null)) as ApiResultResponse<ExperiencePostTemplate>;

                if (checkPostExitance.statusCode != 200)
                {
                    return checkPostExitance;
                }
                var checkUserExitance = await Sender.Send(new GetUserById(request.UserId));
                if (checkUserExitance.statusCode != 200)
                {
                    return checkUserExitance;
                }
                var user = checkUserExitance as ApiResultResponse<TemplateGenericProfile>;

                if (user == null)
                {
                    return new ApiResponse(500, "Invalid user response");
                }


                if (existingLike == null)
                {
                    // 2. Add Logic
                    var newLike = new Likes
                    {
                        UserId = request.UserId,
                        postId = request.postId,
                        CreatedAt = DateTime.UtcNow
                        // Set other properties as needed
                    };

                    await _rWepo.AddAsync(newLike);
                }
                else
                {
                    // 3. Update Logic (e.g., toggling an 'IsDeleted' flag or updating a timestamp)
                    existingLike.UpdatedAt = DateTime.UtcNow;

                    existingLike.LikeType = request.LikeType;

                    await _rWepo.UpdateAsync(existingLike, existingLike.Id);
                }

                // 4. Save changes through Unit of Work
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                var postOwnerId = checkPostExitance.Data.UserPost.Id;

                // prevent self notification
                if (postOwnerId != request.UserId)
                {
                    var reactionText = GetLikeReactionText(request.LikeType);

                    await Sender.Send(
                        new SendLikeNotificationCommand(
                            postOwnerId.ToString(),
                            "New Reaction 🔥",
                            $"{user.Data.Fname} {user.Data.Lname} {reactionText}.",
                            request.postId.ToString()
                        ));
                }


                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                // Log your exception here
                return new ApiResponse(500);
            }
        }
    }
}
