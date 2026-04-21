using Application.Abstraction.message;
using Application.Fetures.Authentication.Command.Models;
using ApplicationBusiness.Fetures.CommentService.Command.Model;
using ApplicationBusiness.Fetures.PostService.Command.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.PostEntity;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.CommentService.Command
{
    
    internal class HiringCommentHandler : ICommandHandler<AddcommentToHiringPost, ApiResponse>
    {
        public ISender Sender { get; set; }

        private IWriteGenericRepo<HiringPostComment> _wpcR;
        private IWriteUnitOfWork _uow;


        public HiringCommentHandler(ISender sender, IWriteGenericRepo<HiringPostComment> wpcR, IWriteUnitOfWork uow)
        {
            Sender = sender;
            _wpcR = wpcR;
            _uow = uow;
        }
        public async Task<ApiResponse> Handle(AddcommentToHiringPost request, CancellationToken cancellationToken)
        {
            var checkPostExitance = await Sender.Send(new IsHiringPostExistCommand(request.postId));

            if(checkPostExitance.statusCode != 200)
            {
                return checkPostExitance;
            }

            var checkUserExitance = await Sender.Send(new IsUserExist(request.Comment.UserId));
            if (checkUserExitance.statusCode != 200)
            {
                return checkUserExitance;
            }

            try
            {

                await _uow.BeginTransactionAsync();
                var item = new HiringPostComment
                {
                    Msg = request.Comment.Msg,
                    HiringPostId = request.postId,
                    UserId = request.Comment.UserId
                };
                await _wpcR.AddAsync(item);
            return new ApiResultResponse<HiringPostComment>((int)StatusCodes.Status201Created,item,"Comment Created");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }


        }
    }
    
    internal class ExperienceCommentHandler : ICommandHandler<AddcommentToExperiencePost, ApiResponse>
    {
        public ISender Sender { get; set; }

        private IWriteGenericRepo<ExperiencePostComment> _wpcR;
        private IWriteUnitOfWork _uow;


        public ExperienceCommentHandler(ISender sender, IWriteGenericRepo<ExperiencePostComment> wpcR, IWriteUnitOfWork uow)
        {
            Sender = sender;
            _wpcR = wpcR;
            _uow = uow;
        }
        public async Task<ApiResponse> Handle(AddcommentToExperiencePost request, CancellationToken cancellationToken)
        {
            var checkPostExitance = await Sender.Send(new IsExperiencePostExistCommand(request.postId));

            if(checkPostExitance.statusCode != 200)
            {
                return checkPostExitance;
            }

            var checkUserExitance = await Sender.Send(new IsUserExist(request.Comment.UserId));
            if (checkUserExitance.statusCode != 200)
            {
                return checkUserExitance;
            }

            try
            {

                await _uow.BeginTransactionAsync();
                var item = new ExperiencePostComment
                {
                    Msg = request.Comment.Msg,
                    ExperiencePostId = request.postId,
                    UserId = request.Comment.UserId
                };
                await _wpcR.AddAsync(item);
            return new ApiResultResponse<ExperiencePostComment>((int)StatusCodes.Status201Created,item,"Comment Created");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }


        }
    }
    
}
