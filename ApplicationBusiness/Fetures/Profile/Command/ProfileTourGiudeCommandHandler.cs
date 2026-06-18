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
    public record AddEarnToTourguide(int TourGuideId, decimal Earn) : ICommand<ApiResponse>;
    internal class ProfileTourGiudeCommandHandler : ICommandHandler<CreateTourGuideProfileCommand, ApiResponse>,
        ICommandHandler<UpdateTourGuideProfileCommand, ApiResponse>,
        ICommandHandler<CheckTourguideExsist, ApiResponse>,
        ICommandHandler<AddEarnToTourguide, ApiResponse>
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

        public async Task<ApiResponse> Handle(CreateTourGuideProfileCommand request, CancellationToken cancellationToken)
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
                        var imageJson =
                            await imageResponse.Content.ReadAsStringAsync(cancellationToken);

                        using var document = JsonDocument.Parse(imageJson);

                        bool isViolent =
                            document.RootElement.GetProperty("is_violent").GetBoolean();

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

                var tourGuide = new TourGuide
                {
                    PhotoUrl = photoUrl,
                    Id = request.Id,
                    UserId = request.Id,
                    Ssn = request.dto.Ssn,
                    Bio = request.dto.Bio,
                    SalaryPerDay = request.dto.SalaryPerDay,
                    City = request.dto.City,
                    Country = request.dto.Country,
                    BuildingNumber = request.dto.BuildingNumber,
                    Street = request.dto.Street
                };

                await _WTR.AddAsync(tourGuide);

                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();

                var tourGuideProfile = new TemplateTourGuide
                {
                    PhotoUrl = tourGuide.PhotoUrl,
                    Id = tourGuide.Id,
                    Ssn = tourGuide.Ssn,
                    Bio = tourGuide.Bio
                };

                return new ApiResultResponse<TemplateTourGuide>(
                    StatusCodes.Status201Created,
                    tourGuideProfile,
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
        
        public async Task<ApiResponse> Handle(UpdateTourGuideProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var tourGuide = await _RTR.GetByIdAsync(request.Id);

                if (tourGuide == null)
                {
                    return new ApiResponse(
                        StatusCodes.Status404NotFound,
                        "Tour guide not found");
                }

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
                        var imageJson =
                            await imageResponse.Content.ReadAsStringAsync(cancellationToken);

                        using var document = JsonDocument.Parse(imageJson);

                        bool isViolent =
                            document.RootElement.GetProperty("is_violent").GetBoolean();

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
                    tourGuide.Ssn = request.dto.Ssn;

                if (!string.IsNullOrEmpty(request.dto.Bio))
                    tourGuide.Bio = request.dto.Bio;

                if (request.dto.SalaryPerDay.HasValue)
                    tourGuide.SalaryPerDay = request.dto.SalaryPerDay.Value;

                if (request.dto.photo is not null)
                {
                    if (!string.IsNullOrEmpty(tourGuide.PhotoUrl))
                        await _cloudinaryService.DeleteFileAsync(tourGuide.PhotoUrl);

                    var photoUrl =
                        await _cloudinaryService.UploadFileAsync(request.dto.photo);

                    tourGuide.PhotoUrl = photoUrl;
                }

                if (!string.IsNullOrEmpty(request.dto.City)
                    && !string.IsNullOrEmpty(request.dto.Country)
                    && !string.IsNullOrEmpty(request.dto.BuildingNumber)
                    && !string.IsNullOrEmpty(request.dto.Street))
                {
                    tourGuide.City = request.dto.City;
                    tourGuide.Country = request.dto.Country;
                    tourGuide.BuildingNumber = request.dto.BuildingNumber;
                    tourGuide.Street = request.dto.Street;
                }

                await _WTR.UpdateAsync(tourGuide, request.Id);

                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();

                var tourGuideProfile = new TemplateTourGuide
                {
                    PhotoUrl = tourGuide.PhotoUrl,
                    Id = tourGuide.Id,
                    Ssn = tourGuide.Ssn,
                    Bio = tourGuide.Bio,
                    BuildingNumber = tourGuide.BuildingNumber,
                    City = tourGuide.City,
                    Street = tourGuide.Street,
                    Country = tourGuide.Country
                };

                if (!string.IsNullOrEmpty(tourGuide.PhotoUrl))
                {
                    var token = await authServ.CreateTokenAsync(tourGuide.Id);

                    return new ApiResultResponse<TemplateTokenTour>(
                        StatusCodes.Status200OK,
                        new TemplateTokenTour
                        {
                            Token = new Token
                            {
                                ExpiryDate = token.Expiration,
                                AccessToken = token.AccessToken,
                                RefreshToken = token.RefreshToken
                            },
                            profile = tourGuideProfile
                        },
                        "Profile updated successfully");
                }

                return new ApiResultResponse<TemplateTourGuide>(
                    StatusCodes.Status200OK,
                    tourGuideProfile,
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


        public async Task<ApiResponse> Handle(CheckTourguideExsist request, CancellationToken cancellationToken)
        {
            if (await _WTR.ExistsAsync(request.TourId))
                return new ApiResponse(StatusCodes.Status302Found);
            return new ApiResponse(StatusCodes.Status404NotFound);

        }

        public async Task<ApiResponse> Handle(AddEarnToTourguide request, CancellationToken cancellationToken)
        {
            try
            {

                var tourGuide = await _RTR.GetByIdAsync(request.TourGuideId);
                await _writeUnitOfWork.BeginTransactionAsync();


                tourGuide.TotalEarnings += request.Earn;

                await _WTR.UpdateAsync(tourGuide, request.TourGuideId);

                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();

                var tourGuideProfile = new TemplateTourGuide
                {
                    PhotoUrl = tourGuide.PhotoUrl,
                    Id = tourGuide.Id,
                    Ssn = tourGuide.Ssn,
                    Bio = tourGuide.Bio,
                    BuildingNumber = tourGuide.BuildingNumber,
                    City = tourGuide.City,
                    Street = tourGuide.Street,
                    Country = tourGuide.Country
                };
                return new ApiResponse(200);
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
