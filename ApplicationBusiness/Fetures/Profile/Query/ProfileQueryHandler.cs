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
    internal class TourGuideProfileQueryHandler : IQueryHandler<GetTourGuideProfileQuery, ApiResponse>
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
                                BookedTrip = item.User.BookingTrips.Select(x => new BookingTripTemplate
                                {
                                    BookingDate = x.BookingDate,
                                    Id = x.Id,
                                    IsPaid = x.IsPaid,
                                    TotalBookingPrice = x.TotalBookingPrice,
                                    TripTilte = x.PublicTrip.Title
                                }).ToList(),
                                PrivateTrips = item.User.CreatedTrips.OfType<PrivateTrip>() // get only private trips
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
                                    CreatedAt = p.CreatedAt,
                                    FullName = $"{item.User.FName} {item.User.LName}",
                                    Description = p.Description,
                                    PhotoUrl = p.PhotoUrl,
                                    Title = p.Title,
                                    City = p.City,
                                    Country = p.Country,
                                    Budget = p.Budget,
                                    TipsAndRecommendations = p.TipsAndRecommendations,
                                    Comments = p.Comments.Select(c => new TemplateComment
                                    {
                                        CreatedAt = c.CreatedAt,
                                        FullName = $"{c.User.FName} {c.User.LName}",
                                        IsEdited = c.IsEdited,
                                        Msg = c.Msg,
                                    }).ToList()

                                }).ToList(),
                                SalaryPerDay = item.SalaryPerDay,
                                Ssn = item.Ssn,
                                Bio = item.Bio,
                                Adresses = item.tourGuidAddresses.Select(s => new Dtos.Profile.Adress
                                {
                                    City = s.City,
                                    Country = s.Country,
                                    BuildingNumber = s.BuildingNumber,
                                    Street = s.Street,
                                }).ToList(),
                                BusinessGalaries = item.tourGuidBusinessGalaries.Select(s => new Dtos.Profile.BusinessGalaryDto
                                {
                                    Date = s.Date,
                                    Description = s.Description,
                                    Location = s.Location,
                                    PhotoUrl = s.PhotoUrl,

                                }).ToList()
                            })
                            .FirstOrDefaultAsync();
            if (temp == null)
                return new ApiResponse((int)HttpStatusCode.NotFound, "there's no profile to user");

            return new ApiResultResponse<TemplateTourGuide>((int)HttpStatusCode.OK, temp);
        }
    }
    internal class TravelerProfileQueryHandler : IQueryHandler<GetTravelerProfileQuery, ApiResponse>
    {
        private IReadGenericRepo<Traveler> _RTR;

        public TravelerProfileQueryHandler(IReadGenericRepo<Traveler> rTR)
        {
            _RTR = rTR;
        }

        public async Task<ApiResponse> Handle(GetTravelerProfileQuery request, CancellationToken cancellationToken)
        {
            var temp = await _RTR.GetAll()
                            .AsNoTracking()
                            .AsSplitQuery()//'IQueryable<Traveler>' does not contain a definition for 'AsSplitQuery' and no accessible extension method 'AsSplitQuery' accepting a first argument of type 'IQueryable<Traveler>' could be found (are you missing a using directive or an assembly reference?)
                            .Where(t => t.Id == request.UserId)
                            .Select(item => new TemplateTraveler
                            {
                                BookedTrip = item.User.BookingTrips.Select(x=>new BookingTripTemplate
                                {
                                    BookingDate = x.BookingDate,
                                    Id = x.Id,
                                    IsPaid = x.IsPaid,
                                    TotalBookingPrice = x.TotalBookingPrice,
                                    TripTilte= x.PublicTrip.Title
                                }).ToList(),
                                PrivateTrips = item.User.CreatedTrips.OfType<PrivateTrip>() // get only private trips
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
                                    CreatedAt = p.CreatedAt,
                                    FullName = $"{item.User.FName} {item.User.LName}",
                                    Description = p.Description,
                                    PhotoUrl = p.PhotoUrl,
                                    Title = p.Title,
                                    City = p.City,
                                    Country = p.Country,
                                    Budget = p.Budget,
                                    TipsAndRecommendations = p.TipsAndRecommendations,
                                    Comments = p.Comments.Select(c => new TemplateComment
                                    {
                                        CreatedAt = c.CreatedAt,
                                        FullName = $"{c.User.FName} {c.User.LName}",
                                        IsEdited = c.IsEdited,
                                        Msg = c.Msg,
                                    }).ToList()

                                }).ToList(),
                                Ssn = item.Ssn,
                                Bio = item.Bio,
                                Adresses = item.trvelerAddresses.Select(s => new Dtos.Profile.Adress
                                {
                                    City = s.City,
                                    Country = s.Country,
                                    BuildingNumber = s.BuildingNumber,
                                    Street = s.Street,
                                }).ToList(),
                            })
                            .FirstOrDefaultAsync();
            if (temp == null)
                return new ApiResponse((int)HttpStatusCode.NotFound, "there's no profile to user");

            return new ApiResultResponse<TemplateTraveler>((int)HttpStatusCode.OK, temp);
        }
    }
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
                    BookedTrip = item.User.BookingTrips.Select(x => new BookingTripTemplate
                    {
                        BookingDate = x.BookingDate,
                        Id = x.Id,
                        IsPaid = x.IsPaid,
                        TotalBookingPrice = x.TotalBookingPrice,
                        TripTilte = x.PublicTrip.Title
                    }).ToList(),
                    PrivateTrips = item.User.CreatedTrips.OfType<PrivateTrip>() // get only private trips
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
                        CreatedAt = p.CreatedAt,
                        FullName = $"{item.User.FName} {item.User.LName}",
                        Description = p.Description,
                        PhotoUrl = p.PhotoUrl,
                        Title = p.Title,
                        City = p.City,
                        Country = p.Country,
                        Budget = p.Budget,
                        TipsAndRecommendations = p.TipsAndRecommendations,
                        Comments = p.Comments.Select(c => new TemplateComment
                        {
                            CreatedAt = c.CreatedAt,
                            FullName = $"{c.User.FName} {c.User.LName}",
                            IsEdited = c.IsEdited,
                            Msg = c.Msg,
                        }).ToList()

                    }).ToList(),
                    Ssn = item.Ssn,
                    Adresses = item.traveleCompanyAddresses.Select(s => new Dtos.Profile.Adress
                    {
                        City = s.City,
                        Country = s.Country,
                        BuildingNumber = s.BuildingNumber,
                        Street = s.Street,
                    }).ToList(),
                    Bio = item.Bio,
                    BusinessGalaries = item.travelCompanyBusinessGalaries.Select(s => new Dtos.Profile.BusinessGalaryDto
                    {
                        Date = s.Date,
                        Description = s.Description,
                        Location = s.Location,
                        PhotoUrl = s.PhotoUrl,

                    }).ToList()
                })
                .FirstOrDefaultAsync();
            if (temp == null)
                return new ApiResponse((int)HttpStatusCode.NotFound, "there's no profile to user");

            return new ApiResultResponse<TemplateTravelComapny>((int)HttpStatusCode.OK, temp);
        }
    }

}
