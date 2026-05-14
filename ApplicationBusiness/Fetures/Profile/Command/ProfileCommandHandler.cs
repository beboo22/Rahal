using Application.Abstraction.message;
using Application.Fetures.Authentication.Command.Models;
using ApplicationBusiness.Abstraction.CloudinaryService;
using ApplicationBusiness.Dtos.Auth;
using ApplicationBusiness.Fetures.Profile.Command.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Profile.Command
{
    internal class ProfileTravelCompanyCommandHandler : ICommandHandler<CreateTravelerCompanyProfileCommand, ApiResponse>,
        ICommandHandler<UpdateTravelerCompanyProfileCommand, ApiResponse>
    {
        private IWriteUnitOfWork _writeUnitOfWork;
        private IWriteGenericRepo<TravelCompany> _WTR;
        private IReadGenericRepo<TravelCompany> _RTR;

        private ICloudinaryService _cloudinaryService;

        public ISender Sender { get; set; }



        public ProfileTravelCompanyCommandHandler(IWriteUnitOfWork writeUnitOfWork, IWriteGenericRepo<TravelCompany> wTR, IReadGenericRepo<TravelCompany> rTR, ISender sender, ICloudinaryService cloudinaryService)
        {
            _writeUnitOfWork = writeUnitOfWork;
            _WTR = wTR;
            _RTR = rTR;
            Sender = sender;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResponse> Handle(CreateTravelerCompanyProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _writeUnitOfWork.BeginTransactionAsync();

                var photourl = await _cloudinaryService.UploadFileAsync(request.dto.Photo);

                //var TravelCompanyBusinessGalaryPhoto = await _cloudinaryService.UploadFileAsync(request.dto.BusinessGalaries.Photo);
                var entity = new TravelCompany
                {
                    PhotoUrl = photourl,

                    Id = request.Id,
                    Ssn = request.dto.Ssn,
                    UserId = request.Id,
                    Bio = request.dto.Bio,


                };
                await _WTR.AddAsync(entity);

                await Sender.Send(new VerifiedUser(request.Id));


                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();
                var temp = new TemplateTravelComapny
                {
                    PhotoUrl = entity.PhotoUrl,
                    Id = entity.Id,
                    Ssn = entity.Ssn,
                    Bio = entity.Bio,
                    BusinessGalaries = entity.travelCompanyBusinessGalaries.Select(s => new Dtos.Profile.BusinessGalaryDto
                    {
                        PhotoUrl = s.PhotoUrl,
                        Date = s.Date,
                        Description = s.Description,
                        Location = s.Location,
                    }).ToList(),
                    BuildingNumber = entity.BuildingNumber,
                    City = entity.City,
                    Street = entity.Street,
                    Country = entity.Country,

                };



                return new ApiResultResponse<TemplateTravelComapny>((int)HttpStatusCode.Created, temp, "Profile created successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        public async Task<ApiResponse> Handle(UpdateTravelerCompanyProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _writeUnitOfWork.BeginTransactionAsync();
                var tComp = await _RTR.GetAll().Include(x => x.travelCompanyBusinessGalaries).FirstOrDefaultAsync(x => x.Id == request.Id);
                if (tComp != null)
                    return new ApiResponse(404, "There's no profile to User");
                if (!string.IsNullOrEmpty(request.dto.Ssn))
                    tComp.Ssn = request.dto.Ssn;
                if (!string.IsNullOrEmpty(request.dto.Bio))
                    tComp.Bio = request.dto.Bio;
                if (request.dto.photo is not null)
                {
                    if (tComp.PhotoUrl != null)
                        await _cloudinaryService.DeleteFileAsync(tComp.PhotoUrl);
                    var photourl = await _cloudinaryService.UploadFileAsync(request.dto.photo);

                    tComp.PhotoUrl = photourl;
                }


                await _WTR.UpdateAsync(tComp, request.Id);

                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();
                var temp = new TemplateTravelComapny
                {
                    Id = tComp.Id,
                    Ssn = tComp.Ssn,
                    Bio = tComp.Bio,
                    BusinessGalaries = tComp.travelCompanyBusinessGalaries.Select(s => new Dtos.Profile.BusinessGalaryDto
                    {
                        PhotoUrl = s.PhotoUrl,
                        Date = s.Date,
                        Description = s.Description,
                        Location = s.Location,
                    }).ToList(),
                    BuildingNumber = tComp.BuildingNumber,
                    City = tComp.City,
                    Street = tComp.Street,
                    Country = tComp.Country,
                };
                return new ApiResultResponse<TemplateTravelComapny>((int)HttpStatusCode.OK, temp, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
    internal class ProfileTourGiudeCommandHandler : ICommandHandler<CreateTourGuideProfileCommand, ApiResponse>,
        ICommandHandler<UpdateTourGuideProfileCommand, ApiResponse>
    {
        IWriteUnitOfWork _writeUnitOfWork;
        IWriteGenericRepo<TourGuide> _WTR;
        IReadGenericRepo<TourGuide> _RTR;
        public ISender Sender { get; set; }

        private ICloudinaryService _cloudinaryService;

        public ProfileTourGiudeCommandHandler(IWriteUnitOfWork writeUnitOfWork, IWriteGenericRepo<TourGuide> wTR, IReadGenericRepo<TourGuide> rTR, ISender sender, ICloudinaryService cloudinaryService)
        {
            _writeUnitOfWork = writeUnitOfWork;
            _WTR = wTR;
            _RTR = rTR;
            Sender = sender;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResponse> Handle(UpdateTourGuideProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _writeUnitOfWork.BeginTransactionAsync();
                var tComp = await _RTR.GetByIdAsync(request.Id);
                //tComp.SalaryPerDay = request.dto.SalaryPerDay;
                if (!string.IsNullOrEmpty(request.dto.Ssn))
                    tComp.Ssn = request.dto.Ssn;
                if (!string.IsNullOrEmpty(request.dto.Bio))
                    tComp.Bio = request.dto.Bio;
                if (request.dto.photo is not null)
                {
                    if (tComp.PhotoUrl != null)
                        await _cloudinaryService.DeleteFileAsync(tComp.PhotoUrl);
                    var photourl = await _cloudinaryService.UploadFileAsync(request.dto.photo);

                    tComp.PhotoUrl = photourl;
                }


                await _WTR.UpdateAsync(tComp, request.Id);

                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();
                var temp = new TemplateTourGuide
                {
                    Id = tComp.Id,
                    SalaryPerDay = tComp.SalaryPerDay,
                    Ssn = tComp.Ssn,
                    Bio = tComp.Bio,
                    BusinessGalaries = tComp.tourGuidBusinessGalaries.Select(s => new Dtos.Profile.BusinessGalaryDto
                    {
                        PhotoUrl = s.PhotoUrl,
                        Date = s.Date,
                        Description = s.Description,
                        Location = s.Location,
                    }).ToList(),
                    BuildingNumber = tComp.BuildingNumber,
                    City = tComp.City,
                    Street = tComp.Street,
                    Country = tComp.Country,
                };
                return new ApiResultResponse<TemplateTourGuide>((int)HttpStatusCode.OK, temp, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(CreateTourGuideProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _writeUnitOfWork.BeginTransactionAsync();

                var photourl = await _cloudinaryService.UploadFileAsync(request.dto.Photo);

                var entity = new TourGuide
                {

                    PhotoUrl = photourl,
                    SalaryPerDay = request.dto.SalaryPerDay,
                    Id = request.Id,
                    Ssn = request.dto.Ssn,
                    UserId = request.Id,
                    Bio = request.dto.Bio
                };
                await _WTR.AddAsync(entity);
                await _writeUnitOfWork.SaveChangesAsync();
                await Sender.Send(new VerifiedUser(request.Id));

                await _writeUnitOfWork.CommitAsync();
                var temp = new TemplateTourGuide
                {
                    PhotoUrl = entity.PhotoUrl,
                    Id = entity.Id,
                    SalaryPerDay = entity.SalaryPerDay,
                    Ssn = entity.Ssn,
                    Bio = entity.Bio,
                    BusinessGalaries = entity.tourGuidBusinessGalaries.Select(s => new Dtos.Profile.BusinessGalaryDto
                    {
                        PhotoUrl = s.PhotoUrl,
                        Date = s.Date,
                        Description = s.Description,
                        Location = s.Location,
                    }).ToList(),
                    BuildingNumber = entity.BuildingNumber,
                    City = entity.City,
                    Street = entity.Street,
                    Country = entity.Country,
                };
                return new ApiResultResponse<TemplateTourGuide>((int)HttpStatusCode.Created, temp, "Profile created successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
    internal class ProfileTravelerCommandHandler : ICommandHandler<CreateTravelerProfileCommand, ApiResponse>,
        ICommandHandler<UpdateTravelerProfileCommand, ApiResponse>
    {
        IWriteUnitOfWork _writeUnitOfWork;
        IWriteGenericRepo<Traveler> _WTR;
        IReadGenericRepo<Traveler> _RTR;
        public ISender Sender { get; set; }
        private ICloudinaryService _cloudinaryService;



        public ProfileTravelerCommandHandler(IWriteGenericRepo<Traveler> wTR, IWriteUnitOfWork writeUnitOfWork, IReadGenericRepo<Traveler> rTR, ISender sender, ICloudinaryService cloudinaryService)
        {
            _WTR = wTR;
            _writeUnitOfWork = writeUnitOfWork;
            _RTR = rTR;
            Sender = sender;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResponse> Handle(UpdateTravelerProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _writeUnitOfWork.BeginTransactionAsync();
                var tComp = await _RTR.GetByIdAsync(request.Id);

                if (!string.IsNullOrEmpty(request.dto.Ssn))
                    tComp.Ssn = request.dto.Ssn;
                if (!string.IsNullOrEmpty(request.dto.Bio))
                    tComp.Bio = request.dto.Bio;
                if (request.dto.photo is not null)
                {
                    if (tComp.PhotoUrl != null)
                        await _cloudinaryService.DeleteFileAsync(tComp.PhotoUrl);
                    var photourl = await _cloudinaryService.UploadFileAsync(request.dto.photo);

                    tComp.PhotoUrl = photourl;
                }


                if (!string.IsNullOrEmpty(request.dto.City)
                    && !string.IsNullOrEmpty(request.dto.Country)
                    && !string.IsNullOrEmpty(request.dto.BuildingNumber)
                    && !string.IsNullOrEmpty(request.dto.Street)
                    )
                {


                    tComp.City = request.dto.City;
                    tComp.Country = request.dto.Country;
                    tComp.BuildingNumber = request.dto.BuildingNumber;
                    tComp.Street = request.dto.Street;
                        
                }



                await _WTR.UpdateAsync(tComp, request.Id);
                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();
                var temp = new TemplateTraveler
                {
                    PhotoUrl = tComp.PhotoUrl,
                    Id = tComp.Id,
                    Ssn = tComp.Ssn,
                    Bio = tComp.Bio,

                    BuildingNumber = tComp.BuildingNumber,
                    City = tComp.City,
                    Street = tComp.Street,
                    Country = tComp.Country,

                };
                return new ApiResultResponse<TemplateTraveler>((int)HttpStatusCode.OK, temp, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(CreateTravelerProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _writeUnitOfWork.BeginTransactionAsync();



                var photourl = await _cloudinaryService.UploadFileAsync(request.dto.Photo);

                var entity = new Traveler
                {
                    PhotoUrl = photourl,
                    Id = request.Id,
                    Ssn = request.dto.Ssn,
                    UserId = request.Id,
                    Bio = request.dto.Bio,

                    City = request.dto.City,
                    Country = request.dto.Country,
                    BuildingNumber = request.dto.BuildingNumber,
                    Street = request.dto.Street,


                };
                await _WTR.AddAsync(entity);
                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();

                var newtoken = await Sender.Send(new VerifiedUser(request.Id)) as ApiResultResponse<UserDto>;
                var temp = new TemplateTokenTraveler();
                var traveler = new TemplateTraveler
                {
                    PhotoUrl = entity.PhotoUrl,
                    Id = entity.Id,
                    Ssn = entity.Ssn,
                    Bio = entity.Bio,
                };
                temp.profile = traveler;
                if (newtoken?.Data?.Token != null)
                    temp.Token = new Token
                    {

                        AccessToken = newtoken.Data.Token.AccessToken,
                        ExpiryDate = newtoken.Data.Token.ExpiryDate,
                        RefreshToken = newtoken.Data.Token.RefreshToken
                    };
                return new ApiResultResponse<TemplateTokenTraveler>((int)HttpStatusCode.Created, temp, "Profile created successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
