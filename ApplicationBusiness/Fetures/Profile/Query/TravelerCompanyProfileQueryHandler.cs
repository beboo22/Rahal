using Application.Abstraction.message;
using ApplicationBusiness.Fetures.BookingTripService.Query.Response;
using ApplicationBusiness.Fetures.PostService.Query.Response;
using ApplicationBusiness.Fetures.Profile.Command;
using ApplicationBusiness.Fetures.Profile.Query.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using Domain.Entity.TripEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApplicationBusiness.Fetures.Profile.Query
{  
    
    internal class TravelerCompanyProfileQueryHandler : IQueryHandler<GetTravelerCompanyProfileQuery, ApiResponse>
    {
        private IReadGenericRepo<TravelCompany> _RTR;

        public TravelerCompanyProfileQueryHandler(IReadGenericRepo<TravelCompany> rTR)
        {
            _RTR = rTR;
        }

        public async Task<ApiResponse> Handle(GetTravelerCompanyProfileQuery request, CancellationToken cancellationToken)
        {
            var temp = await _RTR.GetAll()
                            .AsNoTracking()
                            .Where(t => t.Id == request.UserId)
                .Select(item => new TemplateTravelComapny
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
                            PrifleUser =
                            item.PhotoUrl,

                            FullName = p.CreatedBy.FName + " " + p.CreatedBy.LName,
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
                                PrifleUser = item.PhotoUrl,
                            },
                            IsEdited = c.IsEdited,
                            Msg = c.Msg,
                        }).ToList()

                    }).ToList(),
                    Ssn = item.Ssn,
                    BuildingNumber = item.BuildingNumber,
                    City = item.City,
                    Street = item.Street,
                    Country = item.Country,
                    Bio = item.Bio,
                    //BusinessGalaries = item.travelCompanyBusinessGalaries.Select(s => new Dtos.Profile.BusinessGalaryDto
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

            return new ApiResultResponse<TemplateTravelComapny>((int)HttpStatusCode.OK, temp);
        }
    }
}


