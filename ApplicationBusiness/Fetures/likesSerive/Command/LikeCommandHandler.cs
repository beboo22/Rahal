using Application.Abstraction.message;
using ApplicationBusiness.Fetures.likesSerive.Command.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.PostEntity;
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

        public LikeCommandHandler(IWriteGenericRepo<Likes> repo, IWriteUnitOfWork unitOfWork, IReadGenericRepo<Likes> rRepo)
        {
            _rWepo = repo;
            _unitOfWork = unitOfWork;
            _rRepo = rRepo;
        }

        public async Task<ApiResponse> Handle(AddLike request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Check if the like already exists (assuming a composite key or specific criteria)
                // Adjust the predicate (l => ...) based on your actual Likes entity properties
                var existingLike =  await _rRepo.GetAll().Where(l => l.UserId == request.UserId && l.postId == request.postId).FirstOrDefaultAsync();
                await _unitOfWork.BeginTransactionAsync();
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

                    existingLike.LikeType=request.LikeType;

                    await _rWepo.UpdateAsync(existingLike,existingLike.Id);
                }

                // 4. Save changes through Unit of Work
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

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
