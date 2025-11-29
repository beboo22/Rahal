using Application.Abstraction.message;
using ApplicationBusiness.Fetures.Discount.Query.Models;
using ApplicationBusiness.Fetures.Discount.Query.Responce;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Discount.Query
{
    internal class GenericDiscountQueryHandler :
    IQueryHandler<GetAllGenericDiscountsQuery, ApiResponse>,
    IQueryHandler<GetGenericDiscountByCodeQuery, ApiResponse>
    {
        private readonly IReadGenericRepo<GenericDiscount> _readRepo;

        public GenericDiscountQueryHandler(IReadGenericRepo<GenericDiscount> readRepo)
        {
            _readRepo = readRepo;
        }

        // 🔹 Get All Generic Discounts
        public async Task<ApiResponse> Handle(GetAllGenericDiscountsQuery request, CancellationToken cancellationToken)
        {
            var list = await _readRepo.GetAll()
                                      .AsNoTracking()
                                      .Select(x => new GenericDiscountTemplate
                                      {
                                          Id = x.Id,
                                          Code = x.Code,
                                          DiscountValue = x.DiscountValue,
                                          IsPercentage = x.IsPercentage,
                                          MaxUsage = x.MaxUsage,
                                          CurrentUsage = x.CurrentUsage,
                                          CreatorId = x.CreatorId
                                      })
                                      .ToListAsync();

            return new ApiResultResponse<List<GenericDiscountTemplate>>(200, list);
        }

        // 🔹 Get Generic Discount by Code
        public async Task<ApiResponse> Handle(GetGenericDiscountByCodeQuery request, CancellationToken cancellationToken)
        {
            var temp = await _readRepo.GetAll()
                                      .AsNoTracking()
                                      .Where(x => x.Code == request.Code)
                                      .Select(x => new GenericDiscountTemplate
                                      {
                                          Id = x.Id,
                                          Code = x.Code,
                                          DiscountValue = x.DiscountValue,
                                          IsPercentage = x.IsPercentage,
                                          MaxUsage = x.MaxUsage,
                                          CurrentUsage = x.CurrentUsage,
                                          CreatorId = x.CreatorId
                                      })
                                      .FirstOrDefaultAsync();

            if (temp == null)
                return new ApiResponse(404, "Invalid discount code");

            return new ApiResultResponse<GenericDiscountTemplate>(200, temp);
        }
    }
    internal class SpacificDiscountQueryHandler :
        IQueryHandler<GetAllSpecificDiscountsQuery, ApiResponse>,
        IQueryHandler<GetSpecificDiscountByCodeQuery, ApiResponse>
    {
        private readonly IReadGenericRepo<SpecificDiscount> _readRepo;

        public SpacificDiscountQueryHandler(IReadGenericRepo<SpecificDiscount> readRepo)
        {
            _readRepo = readRepo;
        }

        // 🔹 Get All Specific Discounts (by CreatorId)
        public async Task<ApiResponse> Handle(GetAllSpecificDiscountsQuery request, CancellationToken cancellationToken)
        {
            var list = await _readRepo.GetAll()
                                      .AsNoTracking()
                                      .Where(x => x.CreatorId == request.CreatorId)
                                      .Select(x => new SpecificDiscountTemplate
                                      {
                                          Id = x.Id,
                                          TripId = x.TripId,
                                          Code = x.Code,
                                          DiscountValue = x.DiscountValue,
                                          IsPercentage = x.IsPercentage,
                                          MaxUsage = x.MaxUsage,
                                          CurrentUsage = x.CurrentUsage,
                                          CreatorId = x.CreatorId
                                      })
                                      .ToListAsync();

            return new ApiResultResponse<List<SpecificDiscountTemplate>>(200, list);
        }

        // 🔹 Get Specific Discount by Code
        public async Task<ApiResponse> Handle(GetSpecificDiscountByCodeQuery request, CancellationToken cancellationToken)
        {
            var temp = await _readRepo.GetAll()
                                      .AsNoTracking()
                                      .Where(x => x.Code == request.Code)
                                      .Select(x => new SpecificDiscountTemplate
                                      {
                                          Id = x.Id,
                                          TripId= x.TripId,
                                          Code = x.Code,
                                          DiscountValue = x.DiscountValue,
                                          IsPercentage = x.IsPercentage,
                                          MaxUsage = x.MaxUsage,
                                          CurrentUsage = x.CurrentUsage,
                                          CreatorId = x.CreatorId
                                      })
                                      .FirstOrDefaultAsync();

            if (temp == null)
                return new ApiResponse(404, "Invalid discount code");

            return new ApiResultResponse<SpecificDiscountTemplate>(200, temp);
        }
    }

}
