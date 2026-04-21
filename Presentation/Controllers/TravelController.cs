using ApplicationBusiness.Abstraction.SerpApiService;
using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Dtos.Flights;
using ApplicationBusiness.Dtos.Hotels;
using ApplicationBusiness.Dtos.Profile;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/travel")]
    [Produces("application/json")]
    public class TravelController : ControllerBase
    {
        private readonly ISerpApiService _serpApiService;
        private readonly IReadFlightSearchHistoryRepository _flightHistoryRepo;
        private readonly IWriteFlightSearchHistoryRepository _WflightHistoryRepo;
        private readonly IReadHotelSearchHistoryRepository _hotelHistoryRepo;
        private readonly IWriteHotelSearchHistoryRepository _whotelHistoryRepo;
        private readonly ILogger<TravelController> _logger;

        public TravelController(
            ISerpApiService serpApiService,
            ILogger<TravelController> logger,
            IReadHotelSearchHistoryRepository hotelHistoryRepo,
            IReadFlightSearchHistoryRepository flightHistoryRepo,
            IWriteHotelSearchHistoryRepository whotelHistoryRepo,
            IWriteFlightSearchHistoryRepository wflightHistoryRepo)
        {
            _serpApiService = serpApiService;
            //_flightHistoryRepo = flightHistoryRepo;
            //_hotelHistoryRepo = hotelHistoryRepo;
            _logger = logger;
            _hotelHistoryRepo = hotelHistoryRepo;
            _flightHistoryRepo = flightHistoryRepo;
            _whotelHistoryRepo = whotelHistoryRepo;
            _WflightHistoryRepo = wflightHistoryRepo;
        }

        /// <summary>Search for available flights.</summary>
        [HttpPost("flights/search")]
        [ProducesResponseType(typeof(ApiResultResponse<FlightSearchResponse>), 200)]
        [ProducesResponseType(typeof(ApiResultResponse<object>), 400)]
        public async Task<IActionResult> SearchFlights(
            [FromBody] FlightSearchRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Flight search: {Origin} → {Dest} on {Date}",
                request.DepartureId, request.ArrivalId, request.OutboundDate);

            var result = await _serpApiService.SearchFlightsAsync(request, cancellationToken);

            if (result.statusCode != 200)
                return result.statusCode is not 200
                    ? StatusCode(503, result)
                    : BadRequest(result);

            // Persist search history (fire and forget — don't block response)
            if (result is ApiResultResponse<FlightSearchResponse> response)

            _ = PersistFlightHistoryAsync(response.Data, response.Data?.BestFlights?.Count ?? 0);

            return Ok(result);
        }

        /// <summary>Search for available hotels.</summary>
        [HttpPost("hotels/search")]
        [ProducesResponseType(typeof(ApiResultResponse<HotelSearchResponse>), 200)]
        [ProducesResponseType(typeof(ApiResultResponse<object>), 400)]
        public async Task<IActionResult> SearchHotels(
            [FromBody] HotelSearchRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hotel search: {Dest} {CheckIn}→{CheckOut}",
                request.Destination, request.CheckInDate, request.CheckOutDate);

            var result = await _serpApiService.SearchHotelsAsync(request, cancellationToken);

            if (result.statusCode != 200)
                return result.statusCode is not 200 
                    ? StatusCode(result.statusCode, result)
                    : BadRequest(result);
            if(result is ApiResultResponse<HotelSearchResponse>  response)

            _ = PersistHotelHistoryAsync(response.Data, response.Data?.Properties?.Count ?? 0);

            return Ok(result);
        }

        /// <summary>Retrieve flight search history.</summary>
        [HttpGet("flights/history")]
        [ProducesResponseType(typeof(ApiResultResponse<object>), 200)]
        public async Task<IActionResult> GetFlightHistory(
            [FromQuery] FlightHistoryFilter filter,
            CancellationToken cancellationToken)
        {
            var spec = new FlightSearchHistorySpecification(filter);
            var items = await _flightHistoryRepo.ListAsync(spec, cancellationToken);
            var count = await _flightHistoryRepo.CountAsync(spec, cancellationToken);

            var pagination = new PaginationMeta
            {
                PageIndex = filter.PageIndex,
                PageSize = filter.PageSize,
                TotalCount = count
            };

            return Ok(new ApiResultResponse<object>(200, new
            {
                pagination = pagination,
                items = items
            }, "Flight history retrieved."));
        }

        /// <summary>Retrieve hotel search history.</summary>
        [HttpGet("hotels/history")]
        [ProducesResponseType(typeof(ApiResultResponse<object>), 200)]
        public async Task<IActionResult> GetHotelHistory(
            [FromQuery] HotelHistoryFilter filter,
            CancellationToken cancellationToken)
        {
            var spec = new HotelSearchHistorySpecification(filter);
            var items = await _hotelHistoryRepo.ListAsync(spec, cancellationToken);
            var count = await _hotelHistoryRepo.CountAsync(spec, cancellationToken);

            var pagination = new PaginationMeta
            {
                PageIndex = filter.PageIndex,
                PageSize = filter.PageSize,
                TotalCount = count
            };

            return Ok(new ApiResultResponse<object>(200, new
            {
                pagination = pagination,
                items = items
            }, "Hotel history retrieved."));
        }

        // ─────────── Private Helpers ───────────

        private async Task PersistFlightHistoryAsync(FlightSearchResponse request, int resultCount)
        {
            try
            {
                var bestFlights = request.BestFlights
                    .Select(MapFlightResult)
                    .ToList();

                var otherFlights = request.OtherFlights
                    .Select(MapFlightResult)
                    .ToList();

                var priceInsights = request.PriceInsights == null
                    ? null
                    : new Domain.Entity.Hotel_flights.PriceInsights(
                        request.PriceInsights.LowestPrice,
                        request.PriceInsights.PriceLevel);

                var history = new FlightSearchHistory(
                    bestFlights,
                    otherFlights,
                    priceInsights!,
                    request.SearchId,
                    request.currency);

                await _WflightHistoryRepo.AddAsync(history);
                //await _flightHistoryRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist flight search history.");
            }
        }
        private FlightSegment MapFlightLeg(FlightLeg leg)
        {
            return new FlightSegment(
                new AirportInfo(
                    leg.DepartureAirport.Name,
                    leg.DepartureAirport.Id,
                    DateTime.Parse(leg.DepartureTime)
                ),
                new AirportInfo(
                    leg.ArrivalAirport.Name,
                    leg.ArrivalAirport.Id,
                    DateTime.Parse(leg.ArrivalTime)
                ),
                DateTime.Parse(leg.DepartureTime),
                DateTime.Parse(leg.ArrivalTime),
                leg.Duration,
                leg.Airplane,
                leg.Airline,
                leg.AirlineLogo,
                leg.TravelClass,
                leg.FlightNumber,
                leg.Overnight,
                leg.LegRoom ?? 0
            );
        }
        private FlightOffer MapFlightResult(FlightResult result)
        {
            var segments = result.Flights
                .Select(MapFlightLeg)
                .ToList();

            return new FlightOffer(
                segments,
                result.TotalDuration,
                result.Price,
                result.Type,
                result.Layovers,
                result.CarbonEmissions,
                result.BookingToken
            );
        }

        private async Task PersistHotelHistoryAsync(HotelSearchResponse request, int resultCount)
        {
            try
            {
                var hotels = request.Properties
                    .Select(MapHotelResult)
                    .ToList();

                var brands = request.Brands == null
                    ? null
                    : new HotelBrands();

                var history = new HotelSearchHistory(
                    hotels,
                    request.SearchId,
                    brands!,
                    request.currency);

                await _whotelHistoryRepo.AddAsync(history);
                //await _hotelHistoryRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist hotel search history.");
            }
        }

        private Hotel MapHotelResult(HotelResult hotel)
        {
            var rates = hotel.RatePerNight
                .Select(r => new RatePerNight(
                    r.Lowest,
                    r.BeforeTaxesFees))
                .ToList();

            var location = new Domain.Entity.Hotel_flights.HotelLocation(
                (decimal)hotel.Location.Latitude,
                (decimal)hotel.Location.Longitude);

            return new Hotel(
                hotel.Name,
                hotel.Description,
                hotel.Link,
                (decimal)hotel.Rating,
                hotel.Reviews,
                hotel.Images.Select(i => new HotelImage { Images = i }),
                hotel.LowestPrice,
                hotel.PriceLabel,
                location,
                hotel.NearbyPlaces,
                hotel.PropertyToken,
                hotel.SponsoredHotel,
                hotel.EcoLabel ?? 0,
                rates,
                hotel.Amenities
            );
        }




    }




    [ApiController]
    [Route("api/cache")]
    public class CacheTestController : ControllerBase
    {

        private readonly IDistributedCache _cache;

        public CacheTestController(IDistributedCache cache)
        {
            _cache = cache;
        }

        [HttpPost("set")]
        public async Task<IActionResult> SetCache(string key, string value)
        {
            await _cache.SetStringAsync(
                key,
                value,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return Ok(new
            {
                success = true,
                message = "Cache saved successfully",
                key,
                value
            });
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetCache(string key)
        {
            var value = await _cache.GetStringAsync(key);

            if (value is null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Cache key not found"
                });
            }

            return Ok(new
            {
                success = true,
                key,
                value
            });
        }
    }









}
