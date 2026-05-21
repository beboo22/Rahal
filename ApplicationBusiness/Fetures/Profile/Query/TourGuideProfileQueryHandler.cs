using Application.Abstraction.message;
using ApplicationBusiness.Fetures.Authentication.Query;
using ApplicationBusiness.Fetures.Authentication.Query.Response;
using ApplicationBusiness.Fetures.BookingTripService.Query.Response;
using ApplicationBusiness.Fetures.PostService.Query.Response;
using ApplicationBusiness.Fetures.Profile.Command;
using ApplicationBusiness.Fetures.Profile.Query.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TourGuidEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Profile.Query
{

    public record GetTourgideInSpecCountry(string country) : IQuery<ApiResponse>;
    internal class TourGuideProfileQueryHandler : IQueryHandler<GetTourGuideProfileQuery, ApiResponse>,
        IQueryHandler<GetTourgideInSpecCountry, ApiResponse>
    {
        private IReadGenericRepo<TourGuide> _RTR;

        public TourGuideProfileQueryHandler(IReadGenericRepo<TourGuide> rTR)
        {
            _RTR = rTR;
        }

        public async Task<ApiResponse> Handle(GetTourGuideProfileQuery request, CancellationToken cancellationToken)
        {
            var temp = await _RTR.GetAll()
                            .AsNoTracking()
                            .Where(t => t.Id == request.UserId)
                            .Select(item => new TemplateTourGuide
                            {
                                PhotoUrl = item.PhotoUrl,

                                NumberFollowers = item.User.Followers.Count,
                                NumberFollowing = item.User.Following.Count,
                                Email = item.User.Email,


                                BookedTrip = item.User.BookingPublicTrips.Select(x => new BookingTripTemplate
                                {
                                    BookingDate = x.BookingDate,
                                    Id = x.Id,
                                    IsPaid = x.IsPaid,
                                    TotalBookingPrice = x.TotalBookingPrice,
                                    TripTilte = x.PublicTrip.Title
                                }).ToList(),
                                PrivateTrips = item.User.PrivateTrips // get only private trips
                                            .Select(t => new PrivateTemplateTrip
                                            {
                                                Id = t.Id,
                                                Title = t.Title,
                                                From = t.From,
                                                Destination = t.Destination,
                                                Duration = t.Duration,
                                                Price = t.Price,
                                                StartDate = t.StartDate,
                                                TripCategory = t.TripCategory,
                                                CustomizationFee = t.CustomizationFee
                                            }).ToList(),
                                ExperiencePostTemplates = item.User.Posts.Select(p => new ExperiencePostTemplate
                                {
                                    Id = p.Id,
                                    CreatedAt = p.CreatedAt,

                                    UserPost = new TemplateUserPost
                                    {
                                        Id = p.CreatedBy.Id,
                                        PrifleUser = item.PhotoUrl,

                                        FullName = item.User.FName + " " + item.User.LName,
                                    },


                                    Description = p.Description,
                                    PhotoUrl = p.PhotoUrl,
                                    Title = p.Title,
                                    City = p.City,
                                    Country = p.Country,
                                    //Budget = p.Budget,
                                    //TipsAndRecommendations = p.TipsAndRecommendations,
                                    Comments = p.Comments.Select(c => new TemplateComment
                                    {
                                        CreatedAt = c.CreatedAt,

                                        UserComment = new TemplateUserPost
                                        {
                                            Id = c.User.Id,
                                            FullName = c.User.FName + " " + c.User.LName,
                                            PrifleUser =
                                            c.User.TravelerProfile != null
                                                ? c.User.TravelerProfile.PhotoUrl
                                                : c.User.TourGuideProfile != null
                                                    ? c.User.TourGuideProfile.PhotoUrl
                                                    : null
                                        },

                                        IsEdited = c.IsEdited,
                                        Msg = c.Msg,
                                    }).ToList()

                                }).ToList(),
                                SalaryPerDay = item.SalaryPerDay,
                                Ssn = item.Ssn,
                                Bio = item.Bio,
                                City = item.City,
                                Country = item.Country,
                                BuildingNumber = item.BuildingNumber,
                                Street = item.Street,
                                //BusinessGalaries = item.tourGuidBusinessGalaries.Select(s => new Dtos.Profile.BusinessGalaryDto
                                //{
                                //    Date = s.Date,
                                //    Description = s.Description,
                                //    Location = s.Location,
                                //    PhotoUrl = s.PhotoUrl,

                                //}).ToList()
                            })
                            .FirstOrDefaultAsync();
            if (temp == null)
                return new ApiResponse((int)HttpStatusCode.NotFound, "there's no profile to user");

            return new ApiResultResponse<TemplateTourGuide>((int)HttpStatusCode.OK, temp);
        }

        public async Task<ApiResponse> Handle(GetTourgideInSpecCountry request, CancellationToken cancellationToken)
        {
            var item =await _RTR.GetAll()
                .Where(x => x.Country == request.country)
                .Include(x => x.User)
            .Select(user => new TemplateTourSearch
            {
                Id = user.Id,
                name = user.User.FName+" "+ user.User.LName,
                Email = user.User.Email,
                Photo = user.PhotoUrl
            }).ToListAsync();

            if (!item.Any()) return new ApiResponse(404);

            return new ApiResultResponse<List<TemplateTourSearch>>(200, item);
        }
    }

}
