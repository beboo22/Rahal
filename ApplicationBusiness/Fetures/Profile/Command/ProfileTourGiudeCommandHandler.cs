using Application.Abestraction;
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
    internal class ProfileTourGiudeCommandHandler : ICommandHandler<CreateTourGuideProfileCommand, ApiResponse>,
        ICommandHandler<UpdateTourGuideProfileCommand, ApiResponse>,
        ICommandHandler<CheckTourguideExsist, ApiResponse>
    {
        IWriteUnitOfWork _writeUnitOfWork;
        IWriteGenericRepo<TourGuide> _WTR;
        IReadGenericRepo<TourGuide> _RTR;
        private IAuthentication authServ;

        public ISender Sender { get; set; }

        private ICloudinaryService _cloudinaryService;

        public ProfileTourGiudeCommandHandler(IWriteUnitOfWork writeUnitOfWork, IWriteGenericRepo<TourGuide> wTR, IReadGenericRepo<TourGuide> rTR, ISender sender, ICloudinaryService cloudinaryService, IAuthentication authServ)
        {
            _writeUnitOfWork = writeUnitOfWork;
            _WTR = wTR;
            _RTR = rTR;
            Sender = sender;
            _cloudinaryService = cloudinaryService;
            this.authServ = authServ;
        }

        public async Task<ApiResponse> Handle(UpdateTourGuideProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _writeUnitOfWork.BeginTransactionAsync();
                var tComp = await _RTR.GetByIdAsync(request.Id);

                if (!string.IsNullOrEmpty(request.dto.Ssn))
                    tComp.Ssn = request.dto.Ssn;
                if (!string.IsNullOrEmpty(request.dto.Bio))
                    tComp.Bio = request.dto.Bio;
                if (request.dto.SalaryPerDay.HasValue)
                    tComp.SalaryPerDay = request.dto.SalaryPerDay.Value;
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
                var temp = new TemplateTourGuide
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
                if (tComp.PhotoUrl != null)
                {

                    var token =
                        await authServ.CreateTokenAsync(tComp.Id);
                    return new ApiResultResponse<TemplateTokenTour>((int)HttpStatusCode.OK, new TemplateTokenTour
                    {

                        Token = new Token
                        {
                            ExpiryDate = token.Expiration,
                            AccessToken = token.AccessToken,
                            RefreshToken = token.RefreshToken,
                        },
                        profile = temp
                    }, "Profile updated successfully");
                }

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
                    Id = request.Id,
                    Ssn = request.dto.Ssn,
                    UserId = request.Id,
                    Bio = request.dto.Bio,
                    SalaryPerDay = request.dto.SalaryPerDay,
                    City = request.dto.City,
                    Country = request.dto.Country,
                    BuildingNumber = request.dto.BuildingNumber,
                    Street = request.dto.Street,
                };
                await _WTR.AddAsync(entity);
                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();

                var newtoken = await Sender.Send(new VerifiedUser(request.Id)) as ApiResultResponse<UserDto>;
                var temp = new TemplateTokenTour();
                var traveler = new TemplateTourGuide
                {
                    PhotoUrl = photourl,
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
                return new ApiResultResponse<TemplateTokenTour>((int)HttpStatusCode.Created, temp, "Profile created successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse((int)HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        public async Task<ApiResponse> Handle(CheckTourguideExsist request, CancellationToken cancellationToken)
        {
            if (await _WTR.ExistsAsync(request.TourId))
                return new ApiResponse(StatusCodes.Status302Found);
            return new ApiResponse(StatusCodes.Status404NotFound);

        }
    }
}
