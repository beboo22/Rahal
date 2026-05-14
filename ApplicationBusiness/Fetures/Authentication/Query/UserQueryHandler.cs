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
                Fname = user.FName,
                Lname = user.LName,
                Traveler = UserTemplateMapper.MapTraveler(user),
                TourGuide = UserTemplateMapper.MapTourGuide(user),
                TravelCompany = UserTemplateMapper.MapTravelCompany(user)
            };

            return new ApiResultResponse<TemplateGenericProfile>(200, result);
        }





        public async Task<ApiResponse> Handle(GetStatusForFollowing request, CancellationToken cancellationToken)
        {
            // 1. هنجيب الـ Ids بتاعة الناس اللي اليوزر ده متابعهم الأول
            // ده بيضمن إننا ماسكين الـ IDs الصح
            var followingIds = await _readGenericRepo.GetAll()
                .Where(x => x.Id == request.userId)
                .SelectMany(x => x.Following.Select(f => f.FollowingId)) // FollowingId هو الـ FK للشخص المتابع
                .ToListAsync(cancellationToken);

            if (!followingIds.Any())
                return new ApiResponse(404);

            // 2. دلوقتي هنجيب الـ Statuses بتاعة الـ IDs دي مباشرة
            // هنستخدم الـ Context بتاع الـ Status عشان نضمن إن الداتا تيجي
            var followingsStatuses = await _readGenericRepo.GetAll() // أو استخدم ريبو اليوزر عادي
                .Where(u => followingIds.Contains(u.Id))
                .Select(u => new TemplateStatusOfFollowing
                {
                    UserName = $"{u.FName} {u.LName}",
                    UserPhoto = u.TravelerProfile != null
                                ? u.TravelerProfile.PhotoUrl
                                : (u.TourGuideProfile != null ? u.TourGuideProfile.PhotoUrl : null),

                    UserStatus = u.Status
                        .Select(su => new StatusViewModel
                        {
                            Id = su.Id,
                            Title = su.Title,
                            ItemUrl = su.ItemUrl,
                            CreatedAt = su.CreatedAt
                        }).ToList()
                })
                //.Where(x => x.UserStatus.Any()) // عشان مترجعش يوزرز ملهومش ستاتس حالياً
                .ToListAsync(cancellationToken);

            return new ApiResultResponse<List<TemplateStatusOfFollowing>>(200, followingsStatuses);
        }
    }
}
