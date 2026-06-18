using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.CloudinaryService;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.TravelerCompanyEntity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BusinessGalary.Command
{

    public record DeleteTravelCompanyBusinessGalary(int BusinessGalaryId)
        : ICommand<ApiResponse>;
    public record CreateListTravelCompanyBusinessGalary(
        int TravelCompanyId,
        BusinessGalary BusinessGalary) : ICommand<ApiResponse>;
    public record UpdateTravelCompanyBusinessGalary(
        int BusinessGalaryId,
        BusinessGalary BusinessGalary) : ICommand<ApiResponse>;
    internal class TravelCompanyBusinessGalaryCommand :
    ICommandHandler<CreateListTravelCompanyBusinessGalary, ApiResponse>,
    ICommandHandler<UpdateTravelCompanyBusinessGalary, ApiResponse>,
        ICommandHandler<DeleteTravelCompanyBusinessGalary, ApiResponse>
    {
        private readonly IWriteGenericRepo<TravelCompanyBusinessGalary> _writeGeneric;
        private readonly IReadGenericRepo<TravelCompanyBusinessGalary> _readGeneric;
        private readonly IWriteUnitOfWork _uof;
        private readonly ICloudinaryService _cloudinaryService;

        public TravelCompanyBusinessGalaryCommand(
            IWriteGenericRepo<TravelCompanyBusinessGalary> writeGeneric,
            IReadGenericRepo<TravelCompanyBusinessGalary> readGeneric,
            IWriteUnitOfWork uof,
            ICloudinaryService cloudinaryService)
        {
            _writeGeneric = writeGeneric;
            _readGeneric = readGeneric;
            _uof = uof;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResponse> Handle(
            CreateListTravelCompanyBusinessGalary request,
            CancellationToken cancellationToken)
        {
            try
            {
                using var httpClient = new HttpClient();

                var aiBaseUrl =
                    "https://driven-committees-parade-burner.trycloudflare.com/api/v1";

                // Validate Description
                if (!string.IsNullOrWhiteSpace(request.BusinessGalary.Description))
                {
                    var textPayload = new
                    {
                        text = request.BusinessGalary.Description
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
                        var textJson =
                            await textResponse.Content.ReadAsStringAsync(cancellationToken);

                        using var document = JsonDocument.Parse(textJson);

                        bool isHarmful = document.RootElement
                            .GetProperty("is_harmful")
                            .GetBoolean();

                        if (isHarmful)
                        {
                            return new ApiResponse(
                                StatusCodes.Status400BadRequest,
                                "Description contains harmful content.");
                        }
                    }
                }

                // Validate Image
                if (request.BusinessGalary.PhotoUrl is not null)
                {
                    using var multipartContent = new MultipartFormDataContent();

                    using var stream = request.BusinessGalary.PhotoUrl.OpenReadStream();

                    var fileContent = new StreamContent(stream);

                    fileContent.Headers.ContentType =
                        new MediaTypeHeaderValue(
                            request.BusinessGalary.PhotoUrl.ContentType ?? "image/jpeg");

                    multipartContent.Add(
                        fileContent,
                        "file",
                        request.BusinessGalary.PhotoUrl.FileName);

                    var imageResponse = await httpClient.PostAsync(
                        $"{aiBaseUrl}/toxic-image-classify",
                        multipartContent,
                        cancellationToken);

                    if (imageResponse.IsSuccessStatusCode)
                    {
                        var imageJson =
                            await imageResponse.Content.ReadAsStringAsync(cancellationToken);

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


                await _uof.BeginTransactionAsync();


                string? photoUrl = null;

                if (request.BusinessGalary.PhotoUrl is not null)
                {
                    photoUrl = await _cloudinaryService
                        .UploadFileAsync(request.BusinessGalary.PhotoUrl);
                }

                var travelCompanyBusinessGalary = new TravelCompanyBusinessGalary
                {
                    TravelCompanyId = request.TravelCompanyId,
                    Description = request.BusinessGalary.Description,
                    Location = request.BusinessGalary.Location,
                    Date = request.BusinessGalary.Date,
                    PhotoUrl = photoUrl
                };

                await _writeGeneric.AddAsync(travelCompanyBusinessGalary);

                await _uof.SaveChangesAsync();
                await _uof.CommitAsync();

                return new ApiResponse(
                    StatusCodes.Status201Created,
                    "Business gallery created successfully");
            }
            catch (Exception ex)
            {

                try
                {
                    
                    await _uof.RollbackAsync();
                }
                catch
                {
                }

                return new ApiResponse(
                    StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(
            UpdateTravelCompanyBusinessGalary request,
            CancellationToken cancellationToken)
        {
            try
            {
                using var httpClient = new HttpClient();

                var aiBaseUrl =
                    "https://driven-committees-parade-burner.trycloudflare.com/api/v1";

                if (!string.IsNullOrWhiteSpace(request.BusinessGalary.Description))
                {
                    var textPayload = new
                    {
                        text = request.BusinessGalary.Description
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
                        var textJson =
                            await textResponse.Content.ReadAsStringAsync(cancellationToken);

                        using var document = JsonDocument.Parse(textJson);

                        bool isHarmful = document.RootElement
                            .GetProperty("is_harmful")
                            .GetBoolean();

                        if (isHarmful)
                        {
                            return new ApiResponse(
                                StatusCodes.Status400BadRequest,
                                "Description contains harmful content.");
                        }
                    }
                }

                if (request.BusinessGalary.PhotoUrl is not null)
                {
                    using var multipartContent = new MultipartFormDataContent();

                    using var stream =
                        request.BusinessGalary.PhotoUrl.OpenReadStream();

                    var fileContent = new StreamContent(stream);

                    fileContent.Headers.ContentType =
                        new MediaTypeHeaderValue(
                            request.BusinessGalary.PhotoUrl.ContentType ?? "image/jpeg");

                    multipartContent.Add(
                        fileContent,
                        "file",
                        request.BusinessGalary.PhotoUrl.FileName);

                    var imageResponse = await httpClient.PostAsync(
                        $"{aiBaseUrl}/toxic-image-classify",
                        multipartContent,
                        cancellationToken);

                    if (imageResponse.IsSuccessStatusCode)
                    {
                        var imageJson =
                            await imageResponse.Content.ReadAsStringAsync(cancellationToken);

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

                await _uof.BeginTransactionAsync();

                var businessGalary =
                    await _readGeneric.GetByIdAsync(request.BusinessGalaryId);

                if (businessGalary is null)
                {
                    return new ApiResponse(
                        StatusCodes.Status404NotFound,
                        "Business Gallery not found");
                }

                var photoUrl = businessGalary.PhotoUrl;

                if (request.BusinessGalary.PhotoUrl is not null)
                {
                    photoUrl = await _cloudinaryService
                        .UploadFileAsync(request.BusinessGalary.PhotoUrl);
                }

                //businessGalary.Name = request.BusinessGalary.Name;
                businessGalary.Description = request.BusinessGalary.Description;
                businessGalary.Location = request.BusinessGalary.Location;
                businessGalary.Date = request.BusinessGalary.Date;
                businessGalary.PhotoUrl = photoUrl;

                await _writeGeneric.UpdateAsync(
                    businessGalary,
                    request.BusinessGalaryId);

                await _uof.SaveChangesAsync();
                await _uof.CommitAsync();

                return new ApiResponse(
                    StatusCodes.Status200OK,
                    "Business gallery updated successfully");
            }
            catch (Exception ex)
            {
                try
                {
                    await _uof.RollbackAsync();
                }
                catch
                {
                }

                return new ApiResponse(
                    StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(
        DeleteTravelCompanyBusinessGalary request,
        CancellationToken cancellationToken)
        {
            try
            {
                await _uof.BeginTransactionAsync();

                var businessGalary =
                    await _readGeneric.GetByIdAsync(request.BusinessGalaryId);

                if (businessGalary is null)
                {
                    return new ApiResponse(
                        StatusCodes.Status404NotFound,
                        "Business Gallery not found");
                }

                await _writeGeneric.DeleteAsync(request.BusinessGalaryId);

                if (!string.IsNullOrEmpty(businessGalary.PhotoUrl))
                {
                    await _cloudinaryService.DeleteFileAsync(businessGalary.PhotoUrl);
                }
                await _uof.SaveChangesAsync();
                await _uof.CommitAsync();

                return new ApiResponse(
                    StatusCodes.Status200OK,
                    "Business Gallery deleted successfully");
            }
            catch (Exception ex)
            {
                try
                {
                    await _uof.RollbackAsync();
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
