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
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Profile.Command
{

    internal class ProfileTravelCompanyCommandHandler : ICommandHandler<CreateTravelerCompanyProfileCommand, ApiResponse>,
        ICommandHandler<UpdateTravelerCompanyProfileCommand, ApiResponse>
    {
        private IWriteUnitOfWork _writeUnitOfWork;
        private IWriteGenericRepo<TravelCompany> _WTR;
        private IReadGenericRepo<TravelCompany> _RTR;
        private IAuthentication authServ;


        private ICloudinaryService _cloudinaryService;

        public ISender Sender { get; set; }



        public ProfileTravelCompanyCommandHandler(IWriteUnitOfWork writeUnitOfWork, IWriteGenericRepo<TravelCompany> wTR, IReadGenericRepo<TravelCompany> rTR, ISender sender, ICloudinaryService cloudinaryService, IAuthentication authServ)
        {
            _writeUnitOfWork = writeUnitOfWork;
            _WTR = wTR;
            _RTR = rTR;
            Sender = sender;
            _cloudinaryService = cloudinaryService;
            this.authServ = authServ;
        }
        public async Task<ApiResponse> Handle(CreateTravelerCompanyProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var httpClient = new HttpClient();
                var aiBaseUrl = "https://driven-committees-parade-burner.trycloudflare.com/api/v1";

                // ==========================
                // Validate Bio
                // ==========================
                if (!string.IsNullOrWhiteSpace(request.dto.Bio))
                {
                    var textPayload = new
                    {
                        text = request.dto.Bio
                    };

                    var textContent = new StringContent(
                        JsonSerializer.Serialize(textPayload),
                        Encoding.UTF8,
                        "application/json");

                    var textResponse = await httpClient.PostAsync(
                        $"{aiBaseUrl}/toxic-text-classify",
                        textContent,
                        cancellationToken);

                    if (textResponse.IsSuccessStatusCode)
                    {
                        var textJson = await textResponse.Content.ReadAsStringAsync(cancellationToken);

                        using var document = JsonDocument.Parse(textJson);

                        bool isHarmful = document.RootElement
                            .GetProperty("is_harmful")
                            .GetBoolean();

                        if (isHarmful)
                        {
                            return new ApiResponse(
                                StatusCodes.Status400BadRequest,
                                "Bio contains harmful content.");
                        }
                    }
                }

                // ==========================
                // Validate Image
                // ==========================
                if (request.dto.Photo is not null)
                {
                    using var multipartContent = new MultipartFormDataContent();

                    using var stream = request.dto.Photo.OpenReadStream();

                    var fileContent = new StreamContent(stream);

                    fileContent.Headers.ContentType =
                        new MediaTypeHeaderValue(
                            request.dto.Photo.ContentType ?? "image/jpeg");

                    multipartContent.Add(
                        fileContent,
                        "file",
                        request.dto.Photo.FileName);

                    var imageResponse = await httpClient.PostAsync(
                        $"{aiBaseUrl}/toxic-image-classify",
                        multipartContent,
                        cancellationToken);

                    if (imageResponse.IsSuccessStatusCode)
                    {
                        var imageJson = await imageResponse.Content.ReadAsStringAsync(cancellationToken);

                        using var document = JsonDocument.Parse(imageJson);

                        bool isViolent = document.RootElement
                            .GetProperty("is_violent")
                            .GetBoolean();

                        if (isViolent)
                        {
                            return new ApiResponse(
                                StatusCodes.Status400BadRequest,
                                "Image contains harmful content.");
                        }
                    }
                }

                var photoUrl =
                    await _cloudinaryService.UploadFileAsync(request.dto.Photo);

                await _writeUnitOfWork.BeginTransactionAsync();

                var travelCompany = new TravelCompany
                {
                    PhotoUrl = photoUrl,
                    Id = request.Id,
                    UserId = request.Id,
                    Ssn = request.dto.Ssn,
                    Bio = request.dto.Bio,
                    City = request.dto.City,
                    Country = request.dto.Country,
                    BuildingNumber = request.dto.BuildingNumber,
                    Street = request.dto.Street
                };

                await _WTR.AddAsync(travelCompany);

                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();

                var travelCompanyProfile = new TemplateTravelComapny
                {
                    PhotoUrl = travelCompany.PhotoUrl,
                    Id = travelCompany.Id,
                    Ssn = travelCompany.Ssn,
                    Bio = travelCompany.Bio
                };

                return new ApiResultResponse<TemplateTravelComapny>(
                    StatusCodes.Status201Created,
                    travelCompanyProfile,
                    "Profile created successfully");
            }
            catch (Exception ex)
            {
                try
                {
                    await _writeUnitOfWork.RollbackAsync();
                }
                catch
                {
                }

                return new ApiResponse(
                    StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }
        public async Task<ApiResponse> Handle(UpdateTravelerCompanyProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var travelCompany = await _RTR.GetByIdAsync(request.Id);

                if (travelCompany == null)
                    return new ApiResponse(
                        StatusCodes.Status404NotFound,
                        "Travel company not found");

                using var httpClient = new HttpClient();
                var aiBaseUrl = "https://driven-committees-parade-burner.trycloudflare.com/api/v1";

                // ==========================
                // Validate Bio
                // ==========================
                if (!string.IsNullOrWhiteSpace(request.dto.Bio))
                {
                    var textPayload = new
                    {
                        text = request.dto.Bio
                    };

                    var textContent = new StringContent(
                        JsonSerializer.Serialize(textPayload),
                        Encoding.UTF8,
                        "application/json");

                    var textResponse = await httpClient.PostAsync(
                        $"{aiBaseUrl}/toxic-text-classify",
                        textContent,
                        cancellationToken);

                    if (textResponse.IsSuccessStatusCode)
                    {
                        var textJson = await textResponse.Content.ReadAsStringAsync(cancellationToken);

                        using var document = JsonDocument.Parse(textJson);

                        bool isHarmful = document.RootElement
                            .GetProperty("is_harmful")
                            .GetBoolean();

                        if (isHarmful)
                        {
                            return new ApiResponse(
                                StatusCodes.Status400BadRequest,
                                "Bio contains harmful content.");
                        }
                    }
                }

                // ==========================
                // Validate Image
                // ==========================
                if (request.dto.photo is not null)
                {
                    using var multipartContent = new MultipartFormDataContent();

                    using var stream = request.dto.photo.OpenReadStream();

                    var fileContent = new StreamContent(stream);

                    fileContent.Headers.ContentType =
                        new MediaTypeHeaderValue(
                            request.dto.photo.ContentType ?? "image/jpeg");

                    multipartContent.Add(
                        fileContent,
                        "file",
                        request.dto.photo.FileName);

                    var imageResponse = await httpClient.PostAsync(
                        $"{aiBaseUrl}/toxic-image-classify",
                        multipartContent,
                        cancellationToken);

                    if (imageResponse.IsSuccessStatusCode)
                    {
                        var imageJson = await imageResponse.Content.ReadAsStringAsync(cancellationToken);

                        using var document = JsonDocument.Parse(imageJson);

                        bool isViolent = document.RootElement
                            .GetProperty("is_violent")
                            .GetBoolean();

                        if (isViolent)
                        {
                            return new ApiResponse(
                                StatusCodes.Status400BadRequest,
                                "Image contains harmful content.");
                        }
                    }
                }

                await _writeUnitOfWork.BeginTransactionAsync();

                if (!string.IsNullOrEmpty(request.dto.Ssn))
                    travelCompany.Ssn = request.dto.Ssn;

                if (!string.IsNullOrEmpty(request.dto.Bio))
                    travelCompany.Bio = request.dto.Bio;

                if (request.dto.photo is not null)
                {
                    if (!string.IsNullOrEmpty(travelCompany.PhotoUrl))
                    {
                        await _cloudinaryService.DeleteFileAsync(
                            travelCompany.PhotoUrl);
                    }

                    var photoUrl =
                        await _cloudinaryService.UploadFileAsync(request.dto.photo);

                    travelCompany.PhotoUrl = photoUrl;
                }

                if (!string.IsNullOrEmpty(request.dto.City)
                    && !string.IsNullOrEmpty(request.dto.Country)
                    && !string.IsNullOrEmpty(request.dto.BuildingNumber)
                    && !string.IsNullOrEmpty(request.dto.Street))
                {
                    travelCompany.City = request.dto.City;
                    travelCompany.Country = request.dto.Country;
                    travelCompany.BuildingNumber = request.dto.BuildingNumber;
                    travelCompany.Street = request.dto.Street;
                }

                await _WTR.UpdateAsync(travelCompany, request.Id);

                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();

                var travelCompanyProfile = new TemplateTravelComapny
                {
                    PhotoUrl = travelCompany.PhotoUrl,
                    Id = travelCompany.Id,
                    Ssn = travelCompany.Ssn,
                    Bio = travelCompany.Bio,
                    BuildingNumber = travelCompany.BuildingNumber,
                    City = travelCompany.City,
                    Street = travelCompany.Street,
                    Country = travelCompany.Country
                };

                if (!string.IsNullOrEmpty(travelCompany.PhotoUrl))
                {
                    var token = await authServ.CreateTokenAsync(travelCompany.Id);

                    return new ApiResultResponse<TemplateTokencompany>(
                        StatusCodes.Status200OK,
                        new TemplateTokencompany
                        {
                            Token = new Token
                            {
                                ExpiryDate = token.Expiration,
                                AccessToken = token.AccessToken,
                                RefreshToken = token.RefreshToken
                            },
                            profile = travelCompanyProfile
                        },
                        "Profile updated successfully");
                }

                return new ApiResultResponse<TemplateTravelComapny>(
                    StatusCodes.Status200OK,
                    travelCompanyProfile,
                    "Profile updated successfully");
            }
            catch (Exception ex)
            {
                try
                {
                    await _writeUnitOfWork.RollbackAsync();
                }
                catch
                {
                }

                return new ApiResponse(
                    StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }
    }
   
}
