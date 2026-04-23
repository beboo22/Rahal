using Application.Abstraction.message;
using ApplicationBusiness.Configuration;
using ApplicationBusiness.Dtos.Photos;
using ApplicationBusiness.Fetures.PhotoService.Command.Model;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.photo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PhotoService.Command
{
    internal class SavePhotoCommandHandler : ICommandHandler<SavePhotoCommand, ApiResponse>
    {
        private readonly IWriteGenericRepo<PhotoSearchResponse> _repo; // Your specific photo repo
        private readonly IWriteUnitOfWork _unitOfWork;
        private readonly ILogger<SavePhotoCommandHandler> _logger;
        private readonly SerpApiSettings _settings;
        private readonly IDatabase _cache;

        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public SavePhotoCommandHandler(
            IWriteGenericRepo<PhotoSearchResponse> repo,
            IWriteUnitOfWork unitOfWork,
            ILogger<SavePhotoCommandHandler> logger,
            IOptions<SerpApiSettings> settings,
            IConnectionMultiplexer redis)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _settings = settings.Value;
            _cache = redis.GetDatabase();
        }

        public async Task<ApiResponse> Handle(SavePhotoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Map SerpAPI Response to Domain Entity (PhotoSearchHistory)
                //var entity = MapToHistory(request.Response);
                request.Response.SearchId = request.exactKeyOrgin;
                await _repo.AddAsync(request.Response);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                // ── Caching Logic ──────────────────────────────────────────
                if (_settings.EnableCaching && _cache is not null)
                {
                    var serialized = JsonSerializer.Serialize(request.Response, _jsonOptions);

                    // 1. Set Exact Cache
                    await _cache.StringSetAsync(
                        request.exactKey,
                        serialized,
                        TimeSpan.FromMinutes(_settings.CacheDurationMinutes));

                    // 2. Set Group Cache if null
                    var groupValue = await _cache.StringGetAsync(request.groupKey);
                    if (groupValue.IsNull)
                    {
                        await _cache.StringSetAsync(
                            request.groupKey,
                            serialized,
                            TimeSpan.FromMinutes(_settings.CacheDurationMinutes));
                    }
                }

                return new ApiResultResponse<PhotoSearchResponse>(200, request.Response, "Photos saved and cached.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error saving photo search history");
                return new ApiResponse(500, ex.Message);
            }
        }

        //private static PhotoSearchResponse MapToHistory(SearchPhotoReq response)
        //{
        //    var photos = response.Images.Select(img => PhotoEntity.Create(
        //        img.Title,
        //        img.Source,
        //        img.Thumbnail,
        //        img.Original,
        //        img.Link,
        //        img.Position,
        //        img.OriginalWidth,
        //        img.OriginalHeight
        //    )).ToList();

        //    return new PhotoSearchHistory(photos, response.SearchId);
        //}
    }
}
