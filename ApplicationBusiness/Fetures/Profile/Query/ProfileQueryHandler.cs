using Application.Abstraction.message;
using ApplicationBusiness.Fetures.Profile.Command;
using ApplicationBusiness.Fetures.Profile.Query.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
                            .Where(t => t.Id == request.UserId)
                            .Select(item => new TemplateTraveler
                            {
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
                            .FirstOrDefaultAsync()
                            ;
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
