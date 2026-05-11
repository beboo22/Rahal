using Application.Abstraction.message;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Dtos.Auth;
using ApplicationBusiness.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Fetures.Authentication.Query.Response;
using ApplicationBusiness.Fetures.Profile.Command;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.PostEntity;
using Domain.Entity.TourGuidEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Authentication.Query
{
    internal class UserQueryHandler :
        IQueryHandler<GetUserById, ApiResponse>,
        IQueryHandler<GetStatusForFollowing, ApiResponse>

    {
        private IReadGenericRepo<User> _readGenericRepo;

        public UserQueryHandler(IReadGenericRepo<User> readGenericRepo)
        {
            _readGenericRepo = readGenericRepo;
        }


        public async Task<ApiResponse> Handle(
    GetUserById request,
    CancellationToken cancellationToken)
        {
            var user = await _readGenericRepo.GetByIDSpec(
                new UserSpec(request.UserId, null, null, null));

            if (user == null)
                return new ApiResponse(404, "user not found");

            var result = new TemplateGenericProfile
            {
                Traveler = UserTemplateMapper.MapTraveler(user),
                TourGuide = UserTemplateMapper.MapTourGuide(user),
                TravelCompany = UserTemplateMapper.MapTravelCompany(user)
            };

            return new ApiResultResponse<TemplateGenericProfile>(200, result);
        }





        public async Task<ApiResponse> Handle(GetStatusForFollowing request, CancellationToken cancellationToken)
        {
            // 1. هنجيب اليوزر الحالي ونمشي في سكة الـ Following بتاعته
            var followingsStatuses = await _readGenericRepo.GetAll()
                .Where(x => x.Id == request.userId)
                .SelectMany(x => x.Following) // دخلنا جوه لستة الناس اللي هو متابعهم
                .Select(f => new TemplateStatusOfFollowing
                {
                    // بيانات الشخص اللي أنا متابعه
                    UserName = $"{f.Following.FName} {f.Following.LName}",
                    UserPhoto = f.Following.TravelerProfile != null
                                ? f.Following.TravelerProfile.PhotoUrl
                                : f.Following.TourGuideProfile.PhotoUrl,

                    // هنا بقى بنجيب الـ Statuses بتاعة الشخص ده بس
                    UserStatus = f.Following.StatusUsers
                                    .Select(su => new StatusViewModel
                                    {
                                        Title = su.Status.Title,
                                        ItemUrl = su.Status.ItemUrl,
                                        CreatedAt = su.Status.CreatedAt // يفضل يكون عندك تاريخ عشان ترتبهم
                                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            if (!followingsStatuses.Any())
                return new ApiResponse(404);

            return new ApiResultResponse<List<TemplateStatusOfFollowing>>(200,followingsStatuses);
        }
    }
}
