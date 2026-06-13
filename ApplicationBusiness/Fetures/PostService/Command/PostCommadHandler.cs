using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.CloudinaryService;
using ApplicationBusiness.Fetures.PostService.Command.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.PostEntity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PostService.Command
{
    internal class HiringPostCommadHandler : ICommandHandler<AddHiringPostCommand, ApiResponse>,
        ICommandHandler<UpdateHiringPostCommand, ApiResponse>,
        ICommandHandler<DeleteHiringPostCommand, ApiResponse>,
        ICommandHandler<IsHiringPostExistCommand, ApiResponse>
    {
        private IWriteUnitOfWork _uow { get; set; }

        private IWriteGenericRepo<HiringPost> _WPR;
        private IReadGenericRepo<HiringPost> _RPR;
        public HiringPostCommadHandler(IWriteUnitOfWork uow, IWriteGenericRepo<HiringPost> wPR, IReadGenericRepo<HiringPost> rPR)
        {
            _uow = uow;
            _WPR = wPR;
            _RPR = rPR;
        }

        public async Task<ApiResponse> Handle(AddHiringPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                await _WPR.AddAsync(new HiringPost
                {
                    CreatedById = request.CreatedBy,
                    Status = request.dto.Status,
                    PhotoUrl = request.dto.PhotoUrl,
                    Title = request.dto.Title,
                    Description = request.dto.Description,
                    Requirements = request.dto.Requirements,
                });
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(UpdateHiringPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                var post = await _RPR.GetByIdAsync(request.dto.Id);
                if (post.CreatedById != request.createdBy)
                    return new ApiResponse((int)HttpStatusCode.BadRequest, "User Can't UpdatePost Bec. he is not the create it");
                post.Status = request.dto.Status;
                post.Requirements = request.dto.Requirements;
                post.CreatedById = request.createdBy;
                post.Description = request.dto.Description;
                post.Title = request.dto.Title;
                post.Description = request.dto.Description;
                await _WPR.UpdateAsync(post, post.Id);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(DeleteHiringPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                var post = await _RPR.GetByIdAsync(request.id);
                if (post.CreatedById != request.createdBy)
                    return new ApiResponse((int)HttpStatusCode.BadRequest, "User Can't Delete Post Bec. he is not the create it");
                await _WPR.DeleteAsync(request.id);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500);
            }
        }

        public async Task<ApiResponse> Handle(IsHiringPostExistCommand request, CancellationToken cancellationToken)
        {
            if (await _WPR.ExistsAsync(request.id))
                return new ApiResponse((int)HttpStatusCode.Found, "Post Is Found");
            return new ApiResponse((int)HttpStatusCode.NotFound, "Hiring Post is not found");
        }
    }
    internal class ExperiencePostCommadHandler : ICommandHandler<AddExperiencePostCommand, ApiResponse>,
        ICommandHandler<UpdateExperiencePostCommand, ApiResponse>,
        ICommandHandler<DeleteExperiencePostCommand, ApiResponse>,
        ICommandHandler<IsExperiencePostExistCommand, ApiResponse>

    {


        private IWriteUnitOfWork _uow { get; set; }

        private IWriteExPostRepo _WPR;
        private ICloudinaryService _cloudinaryService;
        private IReadGenericRepo<ExperiencePost> _RPR;
        public ExperiencePostCommadHandler(IWriteUnitOfWork uow, IWriteExPostRepo wPR, IReadGenericRepo<ExperiencePost> rPR, ICloudinaryService cloudinaryService)
        {
            _uow = uow;
            _WPR = wPR;
            _RPR = rPR;
            _cloudinaryService = cloudinaryService;
        }


        public async Task<ApiResponse> Handle(AddExperiencePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Upload the image first to get the PhotoUrl (since we always save)
                string photoUrl = null;
                if (request.dto.Photo != null && request.dto.Photo.Length > 0)
                {
                    photoUrl = await _cloudinaryService.UploadFileAsync(request.dto.Photo);
                }

                // Default status is true, we will flip it to false if any check fails
                bool isPostValid = true;
                using var httpClient = new HttpClient();
                var baseUrl = "https://driven-committees-parade-burner.trycloudflare.com/api/v1";

                // 2. Text Classification Check
                var fullText = $"{request.dto.Title} {request.dto.Description}";
                var textPayload = new { text = fullText };
                var textContent = new StringContent(JsonSerializer.Serialize(textPayload), Encoding.UTF8, "application/json");

                var textResponse = await httpClient.PostAsync($"{baseUrl}/toxic-text-classify", textContent, cancellationToken);
                if (textResponse.IsSuccessStatusCode)
                {
                    var textJson = await textResponse.Content.ReadAsStringAsync(cancellationToken);
                    using var doc = JsonDocument.Parse(textJson);
                    bool isHarmful = doc.RootElement.GetProperty("is_harmful").GetBoolean();

                    if (isHarmful)
                    {
                        isPostValid = false;
                    }
                }

                // 3. Image Classification Check (Only if a photo was uploaded and post is still considered valid)
                if (isPostValid && request.dto.Photo != null && request.dto.Photo.Length > 0)
                {
                    using var multipartContent = new MultipartFormDataContent();
                    using var stream = request.dto.Photo.OpenReadStream();
                    var fileContent = new StreamContent(stream);

                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(request.dto.Photo.ContentType ?? "image/png");
                    multipartContent.Add(fileContent, "file", request.dto.Photo.FileName);

                    var imageResponse = await httpClient.PostAsync($"{baseUrl}/toxic-image-classify", multipartContent, cancellationToken);
                    if (imageResponse.IsSuccessStatusCode)
                    {
                        var imageJson = await imageResponse.Content.ReadAsStringAsync(cancellationToken);
                        using var doc = JsonDocument.Parse(imageJson);
                        bool isViolent = doc.RootElement.GetProperty("is_violent").GetBoolean();

                        if (isViolent)
                        {
                            isPostValid = false;
                        }
                    }
                }

                // 4. Save to Database regardless of the validation outcome
                await _uow.BeginTransactionAsync();

                var item = new ExperiencePost
                {
                    CreatedById = request.CreatedBy,
                    Country = request.dto.Country,
                    PhotoUrl = photoUrl,
                    Title = request.dto.Title,
                    Description = request.dto.Description,
                    City = request.dto.City,
                    IsValid = isPostValid // Set based on AI classification results
                };

                await _WPR.AddAsync(item);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                // 5. Return appropriate response message to the client
                if (!isPostValid)
                {
                    return new ApiResultResponse<ExperiencePost>(
                        StatusCodes.Status202Accepted,
                        item,
                        "Post saved under review due to content policy violations."
                    );
                }

                return new ApiResultResponse<ExperiencePost>(StatusCodes.Status201Created, item, "Post created successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public async Task<ApiResponse> Handle(UpdateExperiencePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                var post = await _RPR.GetByIdAsync(request.dto.Id);
                if (post.CreatedById != request.createdBy)
                    return new ApiResponse((int)HttpStatusCode.BadRequest, "User Can't UpdatePost Bec. he is not the create it");
                post.Country = request.dto.Country;
                post.City = request.dto.City;
                post.CreatedById = request.createdBy;
                post.Description = request.dto.Description;
                post.Title = request.dto.Title;
                post.Description = request.dto.Description;
                await _WPR.UpdateAsync(post, post.Id);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(DeleteExperiencePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                var post = await _RPR.GetByIdAsync(request.id);
                if (post.CreatedById != request.CreatedBy)
                    return new ApiResponse((int)HttpStatusCode.BadRequest, "User Can't Delete Post Bec. he is not the create it");
                await _WPR.DeleteAsync(request.id);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();
                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500);
            }

        }

        public async Task<ApiResponse> Handle(IsExperiencePostExistCommand request, CancellationToken cancellationToken)
        {
            if (await _WPR.ExistsAsync(request.id))
                return new ApiResponse((int)HttpStatusCode.Found, "Post Is Found");
            return new ApiResponse((int)HttpStatusCode.NotFound, "Experience Post is not found");
        }
    }
}
