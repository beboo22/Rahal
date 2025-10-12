using Application.Abstraction.message;
using ApplicationBusiness.Fetures.PostService.Command.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.PostEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PostService.Command
{
    internal class HiringPostCommadHandler : ICommandHandler<AddHiringPostCommand, ApiResponse>,
        ICommandHandler<UpdateHiringPostCommand, ApiResponse>,
        ICommandHandler<DeleteHiringPostCommand, ApiResponse>
    {
        private IWriteUnitOfWork _uow { get; set; }

        private IWriteGenericRepo<HiringPost> _WPR;
        private IReadGenericRepo<HiringPost> _RPR;
        public HiringPostCommadHandler(IWriteUnitOfWork uow, IWriteGenericRepo<HiringPost> wPR, IReadGenericRepo<HiringPost> rPR)
        {
            _uow = uow;
            _WPR = wPR;
            _RPR = rPR;
        }

        public async Task<ApiResponse> Handle(AddHiringPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransiaction();
                await _WPR.AddAsync(new HiringPost
                {
                    CreatedById = request.CreatedBy,
                    Status = request.dto.Status,
                    PhotoUrl = request.dto.PhotoUrl,
                    Title = request.dto.Title,
                    Description = request.dto.Description,
                    Requirements = request.dto.Requirements,
                });
                await _uow.SaveChangesAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(UpdateHiringPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransiaction();
                var post = await _RPR.GetByIdAsync(request.dto.Id);
                if (post.CreatedById != request.createdBy)
                    return new ApiResponse((int)HttpStatusCode.BadRequest, "User Can't UpdatePost Bec. he is not the create it");
                post.Status = request.dto.Status;
                post.Requirements = request.dto.Requirements;
                post.CreatedById = request.createdBy;
                post.Description = request.dto.Description;
                post.Title = request.dto.Title;
                post.Description = request.dto.Description;
                await _WPR.UpdateAsync(post, post.Id);
                await _uow.SaveChangesAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(DeleteHiringPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransiaction();
                var post = await _RPR.GetByIdAsync(request.id);
                if (post.CreatedById != request.createdBy)
                    return new ApiResponse((int)HttpStatusCode.BadRequest, "User Can't Delete Post Bec. he is not the create it");
                await _WPR.DeleteAsync(request.id);
                await _uow.SaveChangesAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500);
            }
        }
    }
    internal class ExperiencePostCommadHandler : ICommandHandler<AddExperiencePostCommand, ApiResponse>,
        ICommandHandler<UpdateExperiencePostCommand, ApiResponse>,
        ICommandHandler<DeleteExperiencePostCommand, ApiResponse>
    {
        private IWriteUnitOfWork _uow { get; set; }

        private IWriteGenericRepo<ExperiencePost> _WPR;
        private IReadGenericRepo<ExperiencePost> _RPR;
        public ExperiencePostCommadHandler(IWriteUnitOfWork uow, IWriteGenericRepo<ExperiencePost> wPR, IReadGenericRepo<ExperiencePost> rPR)
        {
            _uow = uow;
            _WPR = wPR;
            _RPR = rPR;
        }

        public async Task<ApiResponse> Handle(AddExperiencePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransiaction();
                await _WPR.AddAsync(new ExperiencePost
                {
                    CreatedById = request.CreatedBy,
                    Country = request.dto.Country,
                    PhotoUrl = request.dto.PhotoUrl,
                    Title = request.dto.Title,
                    Description = request.dto.Description,
                    City = request.dto.City,
                    Budget = request.dto.Budget,
                    TipsAndRecommendations = request.dto.TipsAndRecommendations,
                });
                await _uow.SaveChangesAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<ApiResponse> Handle(UpdateExperiencePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransiaction();
                var post = await _RPR.GetByIdAsync(request.dto.Id);
                if (post.CreatedById != request.createdBy)
                    return new ApiResponse((int)HttpStatusCode.BadRequest, "User Can't UpdatePost Bec. he is not the create it");
                post.Country = request.dto.Country;
                post.City = request.dto.City;
                post.Budget = request.dto.Budget;
                post.TipsAndRecommendations = request.dto.TipsAndRecommendations;
                post.CreatedById = request.createdBy;
                post.Description = request.dto.Description;
                post.Title = request.dto.Title;
                post.Description = request.dto.Description;
                await _WPR.UpdateAsync(post, post.Id);
                await _uow.SaveChangesAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(DeleteExperiencePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransiaction();
                var post = await _RPR.GetByIdAsync(request.id);
                if (post.CreatedById != request.CreatedBy)
                    return new ApiResponse((int)HttpStatusCode.BadRequest, "User Can't Delete Post Bec. he is not the create it");
                await _WPR.DeleteAsync(request.id);
                await _uow.SaveChangesAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500);
            }

        }
    }
}
