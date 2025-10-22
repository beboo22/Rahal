using ApplicationBusiness.Dtos.Trip;
using ApplicationBusiness.Fetures.TripService.Command.Models;
using ApplicationBusiness.Fetures.TripService.Query.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.BaseResponce;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    //    [Route("api/[controller]")]
    //    [ApiController]
    //    public class TripController : ApiController
    //    {
    //        public TripController(ISender sender) : base(sender) { }

    //        // POST: api/trip
    //        /// <summary>
    //        /// Creates a new public trip.
    //        /// </summary>
    //        /// <param name="dto">Trip details to create.</param>
    //        /// <param name="createdById">ID of the user creating the trip.</param>
    //        /// <returns>The created trip data.</returns>
    //        [ProducesResponseType(typeof(ApiResultResponse<TemplateTrip>), StatusCodes.Status201Created)]
    //        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    //        [HttpPost("Public/{createdById:int}")]
    //        public async Task<IActionResult> CreateTrip([FromBody] AddPublicTripDto dto, int createdById)
    //        {
    //            var result = await Sender.Send(new AddPublicTrip(dto, createdById));
    //            return Ok(result);
    //        }


    //        // DELETE: api/trip/{id}
    //        // POST: api/trip
    //        /// <summary>
    //        /// Delelte public trip.
    //        /// </summary>
    //        /// <param name="dto">Trip details to create.</param>
    //        /// <param name="createdById">ID of the user creating the trip.</param>
    //        /// <returns>The created trip data.</returns>
    //        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    //        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    //        [HttpDelete("Public/{id:int}")]
    //        public async Task<IActionResult> DeleteTrip(int id)
    //        {
    //            var result = await Sender.Send(new DeletePublicTrip(id));
    //            return Ok(result);
    //        }
    //        // POST: api/trip
    //        /// <summary>
    //        /// Creates a new Private trip.
    //        /// </summary>
    //        /// <param name="dto">Trip details to create.</param>
    //        /// <param name="createdById">ID of the user creating the trip.</param>
    //        /// <returns>The created trip data.</returns>
    //        [ProducesResponseType(typeof(ApiResultResponse<TemplateTrip>), StatusCodes.Status201Created)]
    //        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    //        [HttpPost("Private/{createdById:int}")]
    //        public async Task<IActionResult> CreatePrivateTrip([FromBody] AddPrivateTripDto dto, int createdById)
    //        {
    //            var result = await Sender.Send(new AddPrivateTrip(dto, createdById));
    //            return Ok(result);
    //        }

    //        // DELETE: api/trip/{id}
    //        // POST: api/trip
    //        /// <summary>
    //        /// Delete  trip.
    //        /// </summary>
    //        /// <param name="dto">Trip details to create.</param>
    //        /// <param name="createdById">ID of the user creating the trip.</param>
    //        /// <returns>The created trip data.</returns>
    //        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    //        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    //        [HttpDelete("Private/{id:int}")]
    //        public async Task<IActionResult> DeletePrivateTrip(int id)
    //        {
    //            var result = await Sender.Send(new DeletePrivateTrip(id));
    //            return Ok(result);
    //        }
    //    }
    [Route("api/[controller]")]
    [ApiController]
    public class PublicTripController : ApiController
    {
        public PublicTripController(ISender sender) : base(sender) { }

        /// <summary>
        /// Creates a new public trip.
        /// </summary>
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTrip>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("{createdById:int}")]
        public async Task<IActionResult> CreatePublicTrip([FromBody] AddPublicTripDto dto, int createdById)
        {
            var result = await Sender.Send(new AddPublicTrip(dto, createdById));
            return Ok(result);
        }

        /// <summary>
        /// Delete a public trip.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePublicTrip(int id)
        {
            var result = await Sender.Send(new DeletePublicTrip(id));
            return Ok(result);
        }

        /// <summary>
        /// Get all public trips.
        /// </summary>
        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateTrip>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAllTrips()
        {
            var result = await Sender.Send(new GetAllTrip());
            return Ok(result);
        }

        /// <summary>
        /// Search for public trips.
        /// </summary>
        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateTrip>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpPost("Search")]
        public async Task<IActionResult> SearchForTrip([FromBody] SearchForTripDto dto)
        {
            var result = await Sender.Send(new SearchForTrip(dto));
            return Ok(result);
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class PrivateTripController : ApiController
    {
        public PrivateTripController(ISender sender) : base(sender) { }

        /// <summary>
        /// Creates a new private trip.
        /// </summary>
        [ProducesResponseType(typeof(ApiResultResponse<PrivateTemplateTrip>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("{createdById:int}")]
        public async Task<IActionResult> CreatePrivateTrip([FromBody] AddPrivateTripDto dto, int createdById)
        {
            var result = await Sender.Send(new AddPrivateTrip(dto, createdById));
            return Ok(result);
        }

        /// <summary>
        /// Delete a private trip.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePrivateTrip(int id)
        {
            var result = await Sender.Send(new DeletePrivateTrip(id));
            return Ok(result);
        }

        /// <summary>
        /// Get private trips for a specific user.
        /// </summary>
        [ProducesResponseType(typeof(ApiResultResponse<List<PrivateTemplateTrip>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("User/{userId:int}")]
        public async Task<IActionResult> GetPrivateTripsByUserId(int userId)
        {
            var result = await Sender.Send(new GetPrivateTripforUserId(userId));
            return Ok(result);
        }
    }
}
