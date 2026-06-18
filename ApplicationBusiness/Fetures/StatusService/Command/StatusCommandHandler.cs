using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.CloudinaryService;
using ApplicationBusiness.Fetures.StatusService.Command.Model;
using ApplicationBusiness.Fetures.StatusService.Qurey.res;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Status;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.StatusService.Command
{
    internal class StatusCommandHandler:
        ICommandHandler<AddStatus,ApiResponse>,
        ICommandHandler<DeleteStatus,ApiResponse>,
        ICommandHandler<IsStatusExist,ApiResponse>
    {
        private ICloudinaryService _cloudinaryService;

        private IWriteGenericRepo<Status> _writeGenericRepo;
        private IReadGenericRepo<Status> _ReadGenericRepo;
        private IWriteUnitOfWork _unitOfWork;

        public StatusCommandHandler(IWriteGenericRepo<Status> writeGenericRepo, IWriteUnitOfWork unitOfWork, ICloudinaryService cloudinaryService, IReadGenericRepo<Status> readGenericRepo)
        {
            _writeGenericRepo = writeGenericRepo;
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _ReadGenericRepo = readGenericRepo;
        }

        public async Task<ApiResponse> Handle(AddStatus request, CancellationToken cancellationToken)
        {
            try
            {
                using var httpClient = new HttpClient();
                var aiBaseUrl = "https://driven-committees-parade-burner.trycloudflare.com/api/v1";

                // ==========================
                // Validate Title
                // ==========================
                if (!string.IsNullOrWhiteSpace(request.req.Title))
                {
                    var textPayload = new
                    {
                        text = request.req.Title
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
                                "Title contains harmful content.");
                        }
                    }
                }

                // ==========================
                // Validate Image
                // ==========================
                if (request.req.ItemUrl is not null)
                {
                    using var multipartContent = new MultipartFormDataContent();

                    using var stream = request.req.ItemUrl.OpenReadStream();

                    var fileContent = new StreamContent(stream);

                    fileContent.Headers.ContentType =
                        new MediaTypeHeaderValue(
                            request.req.ItemUrl.ContentType ?? "image/jpeg");

                    multipartContent.Add(
                        fileContent,
                        "file",
                        request.req.ItemUrl.FileName);

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

                await _unitOfWork.BeginTransactionAsync();

                var imageUrl =
                    await _cloudinaryService.UploadFileAsync(request.req.ItemUrl);

                var status = new Status
                {
                    ItemUrl = imageUrl,
                    Title = request.req.Title,
                    CreatedById = request.CreatedById,
                };

                await _writeGenericRepo.AddAsync(status);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return new ApiResultResponse<TemplateStatus>(
                    StatusCodes.Status200OK,
                    new TemplateStatus
                    {
                        Id = status.Id,
                        Title = status.Title,
                        ItemUrl = imageUrl
                    });
            }
            catch (Exception ex)
            {
                try
                {
                    await _unitOfWork.RollbackAsync();
                }
                catch
                {
                }

                return new ApiResponse(
                    StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(DeleteStatus request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var item = await _ReadGenericRepo.GetByIdAsync(request.StatusId);

                if (item == null)
                    return new ApiResponse(404);

                if (item.CreatedById != request.CreatedById)
                    return new ApiResponse(403, "forbidden ");



                await _writeGenericRepo.DeleteAsync(item.Id);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500);
            }
        }

        public async Task<ApiResponse> Handle(IsStatusExist request, CancellationToken cancellationToken)
        {
            return new ApiResultResponse<bool>(200, 
                await _writeGenericRepo.ExistsAsync(request.StatusId));
        }
    }





}
