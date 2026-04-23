using ApplicationBusiness.Abstraction.SerpApiService;
using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Dtos.Flights;
using ApplicationBusiness.Dtos.Hotels;
using ApplicationBusiness.Dtos.Photos;
using ApplicationBusiness.Dtos.Profile;
using ApplicationBusiness.Fetures.FlightService.Query.Model;
using ApplicationBusiness.Fetures.HotelService.Query.Model;
using ApplicationBusiness.Fetures.PhotoService.Query.Model;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using Domain.Entity.photo;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/travel")]
    [Produces("application/json")]
    public class TravelController : ApiController
    {

        public TravelController(ISender sender) : base(sender) { }

      

        /// <summary>Search for available flights.</summary>
        [HttpPost("flights/search")]
        [ProducesResponseType(typeof(ApiResultResponse<FlightSearchResponse>), 200)]
        [ProducesResponseType(typeof(ApiResultResponse<object>), 400)]
        public async Task<IActionResult> SearchFlights(
            [FromBody] FlightSearchRequest request,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(
             new SearchFlightOrchestratorQuery(request));

            return StatusCode(result.statusCode, result);

        }

        /// <summary>Search for available hotels.</summary>
        [HttpPost("hotels/search")]
        [ProducesResponseType(typeof(ApiResultResponse<HotelSearchResponse>), 200)]
        [ProducesResponseType(typeof(ApiResultResponse<object>), 400)]
        public async Task<IActionResult> SearchHotels(
            [FromBody] HotelSearchRequest request,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(
            new HotelSearchOrchestratorQuery(request),
            cancellationToken);

            return StatusCode(result.statusCode, result);
        }
        /// <summary>Search for available hotels.</summary>
        [HttpPost("Photo/search")]
        [ProducesResponseType(typeof(ApiResultResponse<PhotoSearchResponse>), 200)]
        [ProducesResponseType(typeof(ApiResultResponse<object>), 400)]
        public async Task<IActionResult> SearchPhoto(
            [FromBody] SearchPhotoReq request,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(
            new PhotoSearchOrchestratorQuery(request),
            cancellationToken);

            return StatusCode(result.statusCode, result);
        }

        ///// <summary>Retrieve flight search history.</summary>
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

        //    return Ok(new ApiResultResponse<object>(200, new
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





    }





    //[ApiController]
    //[Route("api/cache")]
    //public class CacheTestController : ControllerBase
    //{

    //    private readonly IDistributedCache _cache;

    //    public CacheTestController(IDistributedCache cache)
    //    {
    //        _cache = cache;
    //    }

    //    [HttpPost("set")]
    //    public async Task<IActionResult> SetCache(string key, string value)
    //    {
    //        await _cache.SetStringAsync(
    //            key,
    //            value,
    //            new DistributedCacheEntryOptions
    //            {
    //                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    //            });

    //        return Ok(new
    //        {
    //            success = true,
    //            message = "Cache saved successfully",
    //            key,
    //            value
    //        });
    //    }

    //    [HttpGet("get")]
    //    public async Task<IActionResult> GetCache(string key)
    //    {
    //        var value = await _cache.GetStringAsync(key);

    //        if (value is null)
    //        {
    //            return NotFound(new
    //            {
    //                success = false,
    //                message = "Cache key not found"
    //            });
    //        }

    //        return Ok(new
    //        {
    //            success = true,
    //            key,
    //            value
    //        });
    //    }
    //}









}
