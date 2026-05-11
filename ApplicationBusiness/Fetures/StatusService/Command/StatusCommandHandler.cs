using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.CloudinaryService;
using ApplicationBusiness.Fetures.StatusService.Command.Model;
using ApplicationBusiness.Fetures.StatusService.Qurey.res;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.StatusService.Command
{
    internal class StatusCommandHandler:
        ICommandHandler<AddStatus,ApiResponse>,
        ICommandHandler<DeleteStatus,ApiResponse>,
        ICommandHandler<IsStatusExist,ApiResponse>
    {
        private ICloudinaryService _cloudinaryService;

        private IWriteGenericRepo<Status> _writeGenericRepo;
        private IReadGenericRepo<Status> _ReadGenericRepo;
        private IWriteUnitOfWork _unitOfWork;

        public StatusCommandHandler(IWriteGenericRepo<Status> writeGenericRepo, IWriteUnitOfWork unitOfWork, ICloudinaryService cloudinaryService, IReadGenericRepo<Status> readGenericRepo)
        {
            _writeGenericRepo = writeGenericRepo;
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _ReadGenericRepo = readGenericRepo;
        }

        public async Task<ApiResponse> Handle(AddStatus request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();


                var url = await _cloudinaryService.UploadFileAsync(request.req.ItemUrl);


                var item = new Status
                {
                    ItemUrl = url,
                    Title = request.req.Title,
                    CreatedById = request.CreatedById,
                };

                await _writeGenericRepo.AddAsync(
                    item
                );
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return new ApiResultResponse<TemplateStatus>(200,new TemplateStatus { Id = item.Id,Title= item.Title,ItemUrl = url});
            }catch (Exception ex)
            {
                return new ApiResponse(500);
            }
        }

        public async Task<ApiResponse> Handle(DeleteStatus request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var item = await _ReadGenericRepo.GetByIdAsync(request.StatusId);

                if (item == null)
                    return new ApiResponse(404);

                if (item.CreatedById != request.CreatedById)
                    return new ApiResponse(403, "forbidden ");



                await _writeGenericRepo.DeleteAsync(item.Id);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500);
            }
        }

        public async Task<ApiResponse> Handle(IsStatusExist request, CancellationToken cancellationToken)
        {
            return new ApiResultResponse<bool>(200, 
                await _writeGenericRepo.ExistsAsync(request.StatusId));
        }
    }





}
