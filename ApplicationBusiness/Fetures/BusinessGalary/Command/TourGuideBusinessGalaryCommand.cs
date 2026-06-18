using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.CloudinaryService;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.TourGuidEntity;
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
    public record DeleteTourGuideBusinessGalary(int BusinessGalaryId)
    : ICommand<ApiResponse>;

    public record createListTourGuideBusinessGalary(int tourguideId, BusinessGalary tourGuideBusinessGalary) : ICommand<ApiResponse>;
    public record UpdateTourGuideBusinessGalary(int BusinessGalaryId, BusinessGalary tourGuideBusinessGalaries) : ICommand<ApiResponse>;
    public class BusinessGalary
    {
        public string Description { get; set; }
        public string Location { get; set; }
        public DateOnly Date { get; set; }
        public IFormFile? PhotoUrl { get; set; }
    }

    internal class TourGuideBusinessGalaryCommand : ICommandHandler<createListTourGuideBusinessGalary, ApiResponse>,
        ICommandHandler<UpdateTourGuideBusinessGalary, ApiResponse>,
        ICommandHandler<DeleteTourGuideBusinessGalary, ApiResponse>
    {
        private IWriteGenericRepo<TourGuideBusinessGalary> WriteGeneric;
        private IReadGenericRepo<TourGuideBusinessGalary> readGeneric;
        private ICloudinaryService _cloudinaryService;

        private IWriteUnitOfWork _uof { get; set; }

        public TourGuideBusinessGalaryCommand(IWriteGenericRepo<TourGuideBusinessGalary> writeGeneric, IWriteUnitOfWork uof, IReadGenericRepo<TourGuideBusinessGalary> readGeneric, ICloudinaryService cloudinaryService)
        {
            WriteGeneric = writeGeneric;
            _uof = uof;
            this.readGeneric = readGeneric;
            _cloudinaryService = cloudinaryService;
        }
        public async Task<ApiResponse> Handle(createListTourGuideBusinessGalary request, CancellationToken cancellationToken)
        {
            try
            {
                using var httpClient = new HttpClient();

                var aiBaseUrl =
                    "https://driven-committees-parade-burner.trycloudflare.com/api/v1";

                // ==========================
                // Validate Description
                // ==========================
                if (!string.IsNullOrWhiteSpace(request.tourGuideBusinessGalary.Description))
                {
                    var textPayload = new
                    {
                        text = request.tourGuideBusinessGalary.Description
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

                // ==========================
                // Validate Image
                // ==========================
                if (request.tourGuideBusinessGalary.PhotoUrl is not null)
                {
                    using var multipartContent = new MultipartFormDataContent();

                    using var stream = request.tourGuideBusinessGalary.PhotoUrl.OpenReadStream();

                    var fileContent = new StreamContent(stream);

                    fileContent.Headers.ContentType =
                        new MediaTypeHeaderValue(
                            request.tourGuideBusinessGalary.PhotoUrl.ContentType ?? "image/jpeg");

                    multipartContent.Add(
                        fileContent,
                        "file",
                        request.tourGuideBusinessGalary.PhotoUrl.FileName);

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


                await _uof.BeginTransactionAsync();

                string? photoUrl = null;
                if (request.tourGuideBusinessGalary.PhotoUrl is not null)
                {
                    photoUrl = await _cloudinaryService.UploadFileAsync(request.tourGuideBusinessGalary.PhotoUrl);
                }
                var tourGuide = new TourGuideBusinessGalary
                {
                    TourGuidId = request.tourguideId,
                    Description = request.tourGuideBusinessGalary.Description,
                    Location = request.tourGuideBusinessGalary.Location,
                    Date = request.tourGuideBusinessGalary.Date,
                    PhotoUrl = photoUrl
                };

                await WriteGeneric.AddAsync(tourGuide);

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
        public async Task<ApiResponse> Handle(UpdateTourGuideBusinessGalary request, CancellationToken cancellationToken)
        {
            try
            {
                using var httpClient = new HttpClient();

                var aiBaseUrl =
                    "https://driven-committees-parade-burner.trycloudflare.com/api/v1";

                // ==========================
                // Validate Description
                // ==========================
                if (!string.IsNullOrWhiteSpace(request.tourGuideBusinessGalaries.Description))
                {
                    var textPayload = new
                    {
                        text = request.tourGuideBusinessGalaries.Description
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

                // ==========================
                // Validate Image
                // ==========================
                if (request.tourGuideBusinessGalaries.PhotoUrl is not null)
                {
                    using var multipartContent = new MultipartFormDataContent();

                    using var stream =
                        request.tourGuideBusinessGalaries.PhotoUrl.OpenReadStream();

                    var fileContent = new StreamContent(stream);

                    fileContent.Headers.ContentType =
                        new MediaTypeHeaderValue(
                            request.tourGuideBusinessGalaries.PhotoUrl.ContentType ?? "image/jpeg");

                    multipartContent.Add(
                        fileContent,
                        "file",
                        request.tourGuideBusinessGalaries.PhotoUrl.FileName);

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

                await _uof.BeginTransactionAsync();

                var businessGalary =
                    await readGeneric.GetByIdAsync(request.BusinessGalaryId);

                if (businessGalary is null)
                {
                    return new ApiResponse(
                        StatusCodes.Status404NotFound,
                        "Business Gallery not found");
                }

                var photoUrl = businessGalary.PhotoUrl;

                if (request.tourGuideBusinessGalaries.PhotoUrl is not null)
                {
                    photoUrl = await _cloudinaryService.UploadFileAsync(
                        request.tourGuideBusinessGalaries.PhotoUrl);
                }

                businessGalary.Description =
                    request.tourGuideBusinessGalaries.Description;

                businessGalary.Location =
                    request.tourGuideBusinessGalaries.Location;

                businessGalary.Date =
                    request.tourGuideBusinessGalaries.Date;

                businessGalary.PhotoUrl = photoUrl;

                await WriteGeneric.UpdateAsync(
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
        DeleteTourGuideBusinessGalary request,
        CancellationToken cancellationToken)
        {
            try
            {
                await _uof.BeginTransactionAsync();

                var businessGalary =
                    await readGeneric.GetByIdAsync(request.BusinessGalaryId);

                if (businessGalary is null)
                {
                    return new ApiResponse(
                        StatusCodes.Status404NotFound,
                        "Business Gallery not found");
                }

                await WriteGeneric.DeleteAsync(request.BusinessGalaryId);
                if (!string.IsNullOrEmpty(businessGalary.PhotoUrl))
                    await _cloudinaryService.DeleteFileAsync(businessGalary.PhotoUrl);
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
