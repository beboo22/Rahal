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
        //private readonly IFlightSearchHistoryRepository _flightHistoryRepo;
        //private readonly IHotelSearchHistoryRepository _hotelHistoryRepo;
        private readonly ILogger<TravelController> _logger;

        public TravelController(
            ISerpApiService serpApiService,
            //IFlightSearchHistoryRepository flightHistoryRepo,
            //IHotelSearchHistoryRepository hotelHistoryRepo,
            ILogger<TravelController> logger)
        {
            _serpApiService = serpApiService;
            //_flightHistoryRepo = flightHistoryRepo;
            //_hotelHistoryRepo = hotelHistoryRepo;
            _logger = logger;
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

            _ = PersistFlightHistoryAsync(request, response.Data?.BestFlights?.Count ?? 0);

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

            _ = PersistHotelHistoryAsync(request, response.Data?.Properties?.Count ?? 0);

            return Ok(result);
        }

        /// <summary>Retrieve flight search history.</summary>
        //[HttpGet("flights/history")]
        //[ProducesResponseType(typeof(ApiResultResponse<object>), 200)]
        //public async Task<IActionResult> GetFlightHistory(
        //    [FromQuery] FlightHistoryFilter filter,
        //    CancellationToken cancellationToken)
        //{
        //    var spec = new FlightSearchHistorySpecification(filter);
        //    var items = await _flightHistoryRepo.ListAsync(spec, cancellationToken);
        //    var count = await _flightHistoryRepo.CountAsync(spec, cancellationToken);

        //    var pagination = new PaginationMeta
        //    {
        //        PageIndex = filter.PageIndex,
        //        PageSize = filter.PageSize,
        //        TotalCount = count
        //    };

        //    return Ok(new ApiResultResponse<object>(200,new
        //    {
        //        pagination = pagination,
        //        items = items
        //    }, "Flight history retrieved."));
        //}

        /// <summary>Retrieve hotel search history.</summary>
        //[HttpGet("hotels/history")]
        //[ProducesResponseType(typeof(ApiResultResponse<object>), 200)]
        //public async Task<IActionResult> GetHotelHistory(
        //    [FromQuery] HotelHistoryFilter filter,
        //    CancellationToken cancellationToken)
        //{
        //    var spec = new HotelSearchHistorySpecification(filter);
        //    var items = await _hotelHistoryRepo.ListAsync(spec, cancellationToken);
        //    var count = await _hotelHistoryRepo.CountAsync(spec, cancellationToken);

        //    var pagination = new PaginationMeta
        //    {
        //        PageIndex = filter.PageIndex,
        //        PageSize = filter.PageSize,
        //        TotalCount = count
        //    };

        //    return Ok(new ApiResultResponse<object>(200, new
        //    {
        //        pagination = pagination,
        //        items = items
        //    }, "Hotel history retrieved."));
        //}

        // ─────────── Private Helpers ───────────

        private async Task PersistFlightHistoryAsync(FlightSearchRequest request, int resultCount)
        {
            //try
            //{
            //    var history = new FlightSearchHistory
            //    {
            //        UserId = User.Identity?.Name ?? "anonymous",
            //        DepartureId = request.DepartureId,
            //        ArrivalId = request.ArrivalId,
            //        DepartureDate = DateTime.TryParse(request.OutboundDate, out var d) ? d : DateTime.UtcNow,
            //        ReturnDate = DateTime.TryParse(request.ReturnDate, out var r) ? r : null,
            //        TripType = request.TripType,
            //        TravelClass = request.TravelClass,
            //        Adults = request.Adults,
            //        Children = request.Children,
            //        Currency = request.Currency,
            //        ResultCount = resultCount
            //    };
            //    //await _flightHistoryRepo.AddAsync(history);
            //    //await _flightHistoryRepo.SaveChangesAsync();
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Failed to persist flight search history.");
            //}
        }

        private async Task PersistHotelHistoryAsync(HotelSearchRequest request, int resultCount)
        {
            //try
            //{
            //    var history = new HotelSearchHistory
            //    {
            //        UserId = User.Identity?.Name ?? "anonymous",
            //        Destination = request.Destination,
            //        CheckInDate = DateTime.TryParse(request.CheckInDate, out var ci) ? ci : DateTime.UtcNow,
            //        CheckOutDate = DateTime.TryParse(request.CheckOutDate, out var co) ? co : DateTime.UtcNow,
            //        Adults = request.Adults,
            //        Children = request.Children,
            //        Rooms = request.Rooms,
            //        Currency = request.Currency,
            //        //MinRating = request.MinRating,
            //        MinPrice = request.MinPrice,
            //        MaxPrice = request.MaxPrice,
            //        //SortBy = request.SortBy,
            //        ResultCount = resultCount
            //    };
            //    //await _hotelHistoryRepo.AddAsync(history);
            //    //await _hotelHistoryRepo.SaveChangesAsync();
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Failed to persist hotel search history.");
            //}
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
