using ApplicationBusiness.Abstraction.SerpApiService;
using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Dtos.Flights;
using ApplicationBusiness.Dtos.Hotels;
using ApplicationBusiness.Dtos.Photos;
using ApplicationBusiness.Dtos.Profile;
using ApplicationBusiness.Fetures.FlightService.Query;
using ApplicationBusiness.Fetures.FlightService.Query.Model;
using ApplicationBusiness.Fetures.HotelService.Query.Model;
using ApplicationBusiness.Fetures.PhotoService.Query.Model;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using Domain.Entity.photo;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Traveler,TourGuide,TravelerProfileController")]
    public class TravelController : ApiController
    {
        public TravelController(ISender sender) : base(sender) { }

        /// <summary>
        /// Search for available flights via external providers.
        /// </summary>
        [HttpPost("flights/search")]
        [ProducesResponseType(typeof(ApiResultResponse<FlightSearchResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResultResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchFlights(
            [FromBody] FlightSearchRequest request,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new SearchFlightOrchestratorQuery(request), cancellationToken);
            return ProcessResult(result);
        }

        /// <summary>
        /// Search for available hotels via external providers.
        /// </summary>
        [HttpPost("hotels/search")]
        [ProducesResponseType(typeof(ApiResultResponse<HotelSearchResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResultResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchHotels(
            [FromBody] HotelSearchRequest request,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new HotelSearchOrchestratorQuery(request), cancellationToken);
            return ProcessResult(result);
        }

        /// <summary>
        /// Retrieves hotel history or filtered hotel listings from the database.
        /// </summary>
        [HttpGet("hotels/search")]
        [ProducesResponseType(typeof(ApiResultResponse<HotelSearchResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHotels(
            [FromQuery] HotelHistoryFilter request,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new GetHotelsspecQuery(request), cancellationToken);
            return ProcessResult(result);
        }

        /// <summary>
        /// Retrieves flight history or filtered flight offers from the database.
        /// </summary>
        [HttpGet("flight/search")]
        [ProducesResponseType(typeof(ApiResultResponse<HotelSearchResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFlights(
            [FromQuery] FlightHistoryFilter request,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new GetFlightOffer(request), cancellationToken);
            return ProcessResult(result);
        }

        /// <summary>
        /// Search for destination photos.
        /// </summary>
        [HttpPost("Photo/search")]
        [ProducesResponseType(typeof(ApiResultResponse<PhotoSearchResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchPhoto(
            [FromBody] SearchPhotoReq request,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new PhotoSearchOrchestratorQuery(request), cancellationToken);
            return ProcessResult(result);
        }
    }








}
