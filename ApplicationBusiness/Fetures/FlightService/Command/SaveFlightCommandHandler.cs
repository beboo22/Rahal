using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.SerpApiService;
using ApplicationBusiness.Configuration;
using ApplicationBusiness.Dtos.Flights;
using ApplicationBusiness.Fetures.FlightService.Command.Model;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;


namespace Application.Fetures.FlightService.Command
{
    internal class SaveFlightCommandHandler
        : ICommandHandler<SaveFlightCommand, ApiResponse>
    {
        private readonly IWriteFlightSearchHistoryRepository _writeRepo;
        private readonly IWriteUnitOfWork _unitOfWork;
        private readonly ILogger<SaveFlightCommandHandler> _logger;
        private readonly SerpApiSettings _settings;
        //private readonly IDistributedCache? _cache;
        private readonly IDatabase _cache;

        /// <summary>
        /// Options used for Redis cache serialization/deserialization.
        /// PropertyNameCaseInsensitive is enough – do NOT use SnakeCaseLower here
        /// because the domain entity properties are PascalCase and [JsonConstructor]
        /// parameter matching is case-insensitive by default.
        /// </summary>
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            //PropertyNameCaseInsensitive = true,
            PropertyNameCaseInsensitive = true
        };

        public SaveFlightCommandHandler(
            IWriteFlightSearchHistoryRepository writeRepo,
            IWriteUnitOfWork unitOfWork,
            ILogger<SaveFlightCommandHandler> logger,
            IOptions<SerpApiSettings> settings,
            //IDistributedCache? cache
            IConnectionMultiplexer redis
            )
        {
            _writeRepo = writeRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _settings = settings.Value;
            //_cache = cache;
            _cache = redis.GetDatabase();
        }

        public async Task<ApiResponse> Handle(
            SaveFlightCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var history = MapToHistory(request.Response);

                await _writeRepo.AddAsync(history);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                if (_settings.EnableCaching && _cache is not null)
                {
                    var serialized = JsonSerializer.Serialize(history, _jsonOptions);


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

                return new ApiResultResponse<FlightSearchHistory>(200, history);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error saving flight");
                return new ApiResponse(500, ex.Message);
            }
        }

        // ── Mapping ────────────────────────────────────────────────────

        private static FlightSearchHistory MapToHistory(FlightSearchResponse response)
        {
            var bestFlights = response.BestFlights.Select(MapOffer).ToList();
            var otherFlights = response.OtherFlights.Select(MapOffer).ToList();

            var priceInsights = response.PriceInsights == null
                ? null
                : new Domain.Entity.Hotel_flights.PriceInsights(
                    response.PriceInsights.LowestPrice,
                    response.PriceInsights.PriceLevel);

            return new FlightSearchHistory(
                bestFlights,
                otherFlights,
                priceInsights!,
                response.SearchId,
                response.currency);
        }

        private static FlightOffer MapOffer(FlightResult result)
        {
            var segments = result.Flights.Select(MapSegment).ToList();

            return new FlightOffer(
                segments,
                result.TotalDuration,
                result.Price,
                result.Type,
                result.Layovers,
                result.CarbonEmissions,
                result.BookingToken);
        }

        private static FlightSegment MapSegment(FlightLeg leg)
        {
            var departureTime = ParseDate(leg.DepartureAirport.Time);
            var arrivalTime = ParseDate(leg.ArrivalAirport.Time);

            return new FlightSegment(
                // AirportInfo now uses "Code" instead of "Id" to avoid collision
                // with the BaseEntity.Id database key.
                new AirportInfo(leg.DepartureAirport.Name, leg.DepartureAirport.Id, departureTime),
                new AirportInfo(leg.ArrivalAirport.Name, leg.ArrivalAirport.Id, arrivalTime),
                departureTime,
                arrivalTime,
                leg.Duration,
                leg.Airplane,
                leg.Airline,
                leg.AirlineLogo,
                leg.TravelClass,
                leg.FlightNumber,
                leg.Overnight,
                leg.LegRoom ?? 0);
        }

        private static DateTime ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Invalid date value from API");

            var formats = new[]
            {
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-dd HH:mm:ss"
            };

            if (DateTime.TryParseExact(
                    value,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var result))
            {
                return result;
            }

            throw new Exception($"Invalid date format: {value}");
        }
    }
}
