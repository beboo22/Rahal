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

    internal class ProfileTravelerCommandHandler : ICommandHandler<CreateTravelerProfileCommand, ApiResponse>,
        ICommandHandler<UpdateTravelerProfileCommand, ApiResponse>
    {
        IWriteUnitOfWork _writeUnitOfWork;
        IWriteGenericRepo<Traveler> _WTR;
        IReadGenericRepo<Traveler> _RTR;
        private IAuthentication authServ;

        public ISender Sender { get; set; }
        private ICloudinaryService _cloudinaryService;



        public ProfileTravelerCommandHandler(IWriteGenericRepo<Traveler> wTR, IWriteUnitOfWork writeUnitOfWork, IReadGenericRepo<Traveler> rTR, ISender sender, ICloudinaryService cloudinaryService, IAuthentication authServ)
        {
            _WTR = wTR;
            _writeUnitOfWork = writeUnitOfWork;
            _RTR = rTR;
            Sender = sender;
            _cloudinaryService = cloudinaryService;
            this.authServ = authServ;
        }

        public async Task<ApiResponse> Handle(UpdateTravelerProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var tComp = await _RTR.GetByIdAsync(request.Id);

                if (tComp == null)
                    return new ApiResponse(StatusCodes.Status404NotFound, "Traveler not found");

                bool isValid = true;

                using var httpClient = new HttpClient();
                var baseUrl = "https://driven-committees-parade-burner.trycloudflare.com/api/v1";

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
                        $"{baseUrl}/toxic-text-classify",
                        textContent,
                        cancellationToken);

                    if (textResponse.IsSuccessStatusCode)
                    {
                        var textJson = await textResponse.Content.ReadAsStringAsync(cancellationToken);

                        using var doc = JsonDocument.Parse(textJson);

                        bool isHarmful = doc.RootElement
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
                        $"{baseUrl}/toxic-image-classify",
                        multipartContent,
                        cancellationToken);

                    if (imageResponse.IsSuccessStatusCode)
                    {
                        var imageJson = await imageResponse.Content.ReadAsStringAsync(cancellationToken);

                        using var doc = JsonDocument.Parse(imageJson);

                        bool isViolent = doc.RootElement
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

                // ==========================
                // Start Transaction
                // ==========================
                await _writeUnitOfWork.BeginTransactionAsync();

                if (!string.IsNullOrEmpty(request.dto.Ssn))
                    tComp.Ssn = request.dto.Ssn;

                if (!string.IsNullOrEmpty(request.dto.Bio))
                    tComp.Bio = request.dto.Bio;

                if (request.dto.photo is not null)
                {
                    if (!string.IsNullOrEmpty(tComp.PhotoUrl))
                        await _cloudinaryService.DeleteFileAsync(tComp.PhotoUrl);

                    var photoUrl =
                        await _cloudinaryService.UploadFileAsync(request.dto.photo);

                    tComp.PhotoUrl = photoUrl;
                }

                if (!string.IsNullOrEmpty(request.dto.City)
                    && !string.IsNullOrEmpty(request.dto.Country)
                    && !string.IsNullOrEmpty(request.dto.BuildingNumber)
                    && !string.IsNullOrEmpty(request.dto.Street))
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
                    Country = tComp.Country
                };

                if (!string.IsNullOrEmpty(tComp.PhotoUrl))
                {
                    var token = await authServ.CreateTokenAsync(tComp.Id);

                    return new ApiResultResponse<TemplateTokenTraveler>(
                        StatusCodes.Status200OK,
                        new TemplateTokenTraveler
                        {
                            Token = new Token
                            {
                                ExpiryDate = token.Expiration,
                                AccessToken = token.AccessToken,
                                RefreshToken = token.RefreshToken
                            },
                            profile = temp
                        },
                        "Profile updated successfully");
                }

                return new ApiResultResponse<TemplateTraveler>(
                    StatusCodes.Status200OK,
                    temp,
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

        public async Task<ApiResponse> Handle(CreateTravelerProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var httpClient = new HttpClient();
                var baseUrl = "https://driven-committees-parade-burner.trycloudflare.com/api/v1";

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
                        $"{baseUrl}/toxic-text-classify",
                        textContent,
                        cancellationToken);

                    if (textResponse.IsSuccessStatusCode)
                    {
                        var textJson = await textResponse.Content.ReadAsStringAsync(cancellationToken);

                        using var doc = JsonDocument.Parse(textJson);

                        bool isHarmful = doc.RootElement
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
                        $"{baseUrl}/toxic-image-classify",
                        multipartContent,
                        cancellationToken);

                    if (imageResponse.IsSuccessStatusCode)
                    {
                        var imageJson = await imageResponse.Content.ReadAsStringAsync(cancellationToken);

                        using var doc = JsonDocument.Parse(imageJson);

                        bool isViolent = doc.RootElement
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

                // ==========================
                // Upload Image After Validation
                // ==========================
                var photoUrl = await _cloudinaryService
                    .UploadFileAsync(request.dto.Photo);

                // ==========================
                // Save Profile
                // ==========================
                await _writeUnitOfWork.BeginTransactionAsync();

                var entity = new Traveler
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

                await _WTR.AddAsync(entity);

                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();

                var newtoken =
                    await Sender.Send(new VerifiedUser(request.Id))
                    as ApiResultResponse<UserDto>;

                var traveler = new TemplateTraveler
                {
                    PhotoUrl = entity.PhotoUrl,
                    Id = entity.Id,
                    Ssn = entity.Ssn,
                    Bio = entity.Bio,
                    City = entity.City,
                    Country = entity.Country,
                    BuildingNumber = entity.BuildingNumber,
                    Street = entity.Street
                };

                var response = new TemplateTokenTraveler
                {
                    profile = traveler
                };

                if (newtoken?.Data?.Token != null)
                {
                    response.Token = new Token
                    {
                        AccessToken = newtoken.Data.Token.AccessToken,
                        RefreshToken = newtoken.Data.Token.RefreshToken,
                        ExpiryDate = newtoken.Data.Token.ExpiryDate
                    };
                }

                return new ApiResultResponse<TemplateTokenTraveler>(
                    StatusCodes.Status201Created,
                    response,
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
    }
}
