using Application.Abstraction.message;
using ApplicationBusiness.Configuration;
using ApplicationBusiness.Dtos.Hotels;
using ApplicationBusiness.Fetures.HotelService.Command.Model;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.HotelService.Command
{

    internal class SaveHotelCommandHandler
        : ICommandHandler<SaveHotelCommand, ApiResponse>
    {
        private readonly IWriteHotelSearchHistoryRepository _repo;
        private readonly IWriteUnitOfWork _unitOfWork;
        private readonly ILogger<SaveHotelCommandHandler> _logger;
        private readonly SerpApiSettings _settings;
        //private readonly IDistributedCache? _cache;
        private readonly IDatabase _cache;

        /// <summary>
        /// Options used for Redis cache serialization/deserialization.
        /// PropertyNameCaseInsensitive lets STJ match "lowestPrice" → "LowestPrice" etc.
        /// We do NOT use SnakeCaseLower here because the domain entity properties are
        /// PascalCase and the [JsonConstructor] parameters match them case-insensitively.
        /// </summary>
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            // Do NOT set PropertyNamingPolicy – keep default PascalCase so the
            // [JsonConstructor] parameter matching works cleanly.
        };

        public SaveHotelCommandHandler(
            IWriteHotelSearchHistoryRepository repo,
            IWriteUnitOfWork unitOfWork,
            ILogger<SaveHotelCommandHandler> logger,
            IOptions<SerpApiSettings> settings,
            //IDistributedCache? cache
            IConnectionMultiplexer redis
            )
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _settings = settings.Value;
            //_cache = cache;
            _cache = redis.GetDatabase();

        }

        public async Task<ApiResponse> Handle(
            SaveHotelCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var entity = MapToHistory(request.Response);

                await _repo.AddAsync(entity);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                if (_settings.EnableCaching && _cache is not null)
                {
                    var serialized = JsonSerializer.Serialize(entity, _jsonOptions);

                    //await _cache.SetStringAsync(request.exactKey, serialized, new DistributedCacheEntryOptions
                    //{
                    //    AbsoluteExpirationRelativeToNow =
                    //            TimeSpan.FromMinutes(_settings.CacheDurationMinutes)
                    //}, cancellationToken);

                    //if (await _cache.GetStringAsync(request.groupKey, cancellationToken) is null)
                    //{
                    //    await _cache.SetStringAsync(request.groupKey, serialized, new DistributedCacheEntryOptions
                    //    {
                    //        AbsoluteExpirationRelativeToNow =
                    //            TimeSpan.FromMinutes(_settings.CacheDurationMinutes)
                    //    }, cancellationToken);
                    //}



                    await _cache.StringSetAsync(
                                request.exactKey,
                                serialized,
                                TimeSpan.FromMinutes(_settings.CacheDurationMinutes)
                            );

                    // 2. Check and set the group cache item
                    var groupValue = await _cache.StringGetAsync(request.groupKey);
                    if (groupValue.IsNull)
                    {
                        await _cache.StringSetAsync(
                            request.groupKey,
                            serialized,
                            TimeSpan.FromMinutes(_settings.CacheDurationMinutes)
                        );
                    }


                    //await _cache.SetStringAsync(
                    //    request.CacheKey,
                    //    serialized,
                    //    new DistributedCacheEntryOptions
                    //    {
                    //        AbsoluteExpirationRelativeToNow =
                    //            TimeSpan.FromMinutes(_settings.CacheDurationMinutes)
                    //    },
                    //    cancellationToken);
                }

                return new ApiResultResponse<HotelSearchHistory>(200, entity);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error saving hotel history");
                return new ApiResponse(500, ex.Message);
            }
        }

        // ── Mapping ────────────────────────────────────────────────────

        private static HotelSearchHistory MapToHistory(HotelSearchResponse response)
        {
            var hotels = response.Properties
                .Select(MapHotelResult)
                .ToList();

            return new HotelSearchHistory(
                hotels,
                response.SearchId,
                response.currency);
        }

        private static Hotel MapHotelResult(HotelResult hotel)
        {
            var rates = hotel.RatePerNight?
                .Select(r => new RatePerNight(r.Lowest, r.BeforeTaxesFees))
                .ToList()
                ?? new List<RatePerNight>();

            var location = new Domain.Entity.Hotel_flights.HotelLocation(
                (decimal)(hotel.Location?.Latitude ?? 0),
                (decimal)(hotel.Location?.Longitude ?? 0));

            // Use Hotel.Create() – it accepts IEnumerable<string> for images/amenities/
            // nearbyPlaces and converts them to the comma-separated strings the entity stores.
            return Hotel.Create(
                hotel.Name,
                hotel.Description,
                hotel.Link,
                (decimal)hotel.Rating,
                hotel.Reviews,
                hotel.Images,
                hotel.LowestPrice,
                hotel.PriceLabel,
                location,
                hotel.NearbyPlaces,
                hotel.PropertyToken,
                hotel.SponsoredHotel,
                hotel.EcoLabel ?? 0,
                rates,
                hotel.Amenities);
        }
    }

}
