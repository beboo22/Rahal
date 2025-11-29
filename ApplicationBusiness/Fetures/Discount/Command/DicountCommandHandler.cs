using Application.Abstraction.message;
using ApplicationBusiness.Fetures.Discount.Command.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Discount.Command
{
    internal class SpecificDiscountCommandHandler :
    ICommandHandler<CreateSpecificDiscountCommand, ApiResponse>,
    ICommandHandler<UpdateSpecificDiscountCommand, ApiResponse>
    {
        private readonly IWriteUnitOfWork _uow;
        private readonly IWriteGenericRepo<SpecificDiscount> _writeRepo;
        private readonly IReadGenericRepo<SpecificDiscount> _readRepo;

        public SpecificDiscountCommandHandler(
            IWriteUnitOfWork uow,
            IWriteGenericRepo<SpecificDiscount> writeRepo,
            IReadGenericRepo<SpecificDiscount> readRepo)
        {
            _uow = uow;
            _writeRepo = writeRepo;
            _readRepo = readRepo;
        }

        public async Task<ApiResponse> Handle(CreateSpecificDiscountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransiaction();

                var entity = new SpecificDiscount
                {
                    TripId = request.dto.TripId,
                    CreatorId = request.dto.CreatorId,
                    DiscountValue = request.dto.DiscountValue,
                    IsPercentage = request.dto.IsPercentage,
                    MaxUsage = request.dto.MaxUsage
                };

                await _writeRepo.AddAsync(entity);
                await _uow.SaveChangesAsync();

                return new ApiResultResponse<SpecificDiscount>((int)HttpStatusCode.Created, entity, "Specific discount created");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(UpdateSpecificDiscountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransiaction();

                var discount = await _readRepo.GetByIdAsync(request.Id);
                if (discount == null)
                    return new ApiResponse(404, "Discount not found");

                discount.DiscountValue = request.dto.DiscountValue;
                discount.IsPercentage = request.dto.IsPercentage;
                discount.MaxUsage = request.dto.MaxUsage;

                await _writeRepo.UpdateAsync(discount, discount.Id);
                await _uow.SaveChangesAsync();

                return new ApiResultResponse<SpecificDiscount>(200, discount, "Specific discount updated");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
    }



    internal class GenericDiscountCommandHandler :
     ICommandHandler<CreateGenericDiscountCommand, ApiResponse>,
     ICommandHandler<UpdateGenericDiscountCommand, ApiResponse>
    {
        private readonly IWriteUnitOfWork _uow;
        private readonly IWriteGenericRepo<GenericDiscount> _writeRepo;
        private readonly IReadGenericRepo<GenericDiscount> _readRepo;

        public GenericDiscountCommandHandler(
            IWriteUnitOfWork uow,
            IWriteGenericRepo<GenericDiscount> writeRepo,
            IReadGenericRepo<GenericDiscount> readRepo)
        {
            _uow = uow;
            _writeRepo = writeRepo;
            _readRepo = readRepo;
        }

        public async Task<ApiResponse> Handle(CreateGenericDiscountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransiaction();

                var entity = new GenericDiscount
                {
                    CreatorId = request.dto.CreatorId,
                    DiscountValue = request.dto.DiscountValue,
                    IsPercentage = request.dto.IsPercentage,
                    MaxUsage = request.dto.MaxUsage,
                    CurrentUsage = 0
                };

                await _writeRepo.AddAsync(entity);
                await _uow.SaveChangesAsync();

                return new ApiResultResponse<GenericDiscount>((int)HttpStatusCode.Created, entity, "Generic discount created");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(UpdateGenericDiscountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransiaction();

                var discount = await _readRepo.GetByIdAsync(request.Id);
                if (discount == null)
                    return new ApiResponse(404, "Discount not found");

                discount.DiscountValue = request.dto.DiscountValue;
                discount.IsPercentage = request.dto.IsPercentage;
                discount.MaxUsage = request.dto.MaxUsage;

                await _writeRepo.UpdateAsync(discount, request.Id);
                await _uow.SaveChangesAsync();

                return new ApiResultResponse<GenericDiscount>(200, discount, "Generic discount updated");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
    }

}
