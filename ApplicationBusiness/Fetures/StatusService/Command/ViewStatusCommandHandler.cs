using Application.Abstraction.message;
using ApplicationBusiness.Fetures.StatusService.Command.Model;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Status;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.StatusService.Command
{
    internal class ViewStatusCommandHandler : ICommandHandler<ViewStatus, ApiResponse>,
        ICommandHandler<LoveStatus, ApiResponse>
    {
        private IWriteStatusUserRepo _repo;
        private IWriteUnitOfWork _unitOfWork;
        public ViewStatusCommandHandler(IWriteStatusUserRepo repo, IWriteUnitOfWork unitOfWork, IMediator mediator)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }
        private readonly IMediator _mediator;


        public async Task<ApiResponse> Handle(ViewStatus request, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _mediator.Send(new IsStatusExist(request.StatusId)) as ApiResultResponse<bool>;

                if (item.Data == false)
                    return new ApiResponse(404,"Status Not Found");

                await _unitOfWork.BeginTransactionAsync();
                await _repo.AddAsync(new StatusUser
                {
                    StatusId = request.StatusId,
                    viewById = request.ViewerId
                });
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return new ApiResponse(200, "viewed success");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new ApiResponse(500);
            }

        }

        public async Task<ApiResponse> Handle(LoveStatus request, CancellationToken cancellationToken)
        {

            try
            {
                var item = await _mediator.Send(new IsStatusExist(request.StatusId)) as ApiResultResponse<bool>;

                if (item.Data == false)
                    return new ApiResponse(404, "Status Not Found");

                await _unitOfWork.BeginTransactionAsync();
                if (!await _repo.ExistsAsync(request.ViewerId, request.StatusId))
                {


                    await _repo.AddAsync(new StatusUser
                    {
                        StatusId = request.StatusId,
                        viewById = request.ViewerId,
                        Isloved = request.love
                    });
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    await _repo.UpdateAsync(new StatusUser
                    {
                        StatusId = request.StatusId,
                        viewById = request.ViewerId,
                        Isloved = request.love
                    },0);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();

                }


                return new ApiResponse(200, "viewed success");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new ApiResponse(500);
            }

        }
    }
}
