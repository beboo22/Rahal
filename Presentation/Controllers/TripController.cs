using ApplicationBusiness.Dtos.Trip;
using ApplicationBusiness.Fetures.TripService.Command.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.BaseResponce;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ApiController
    {
        public TripController(ISender sender) : base(sender) { }

        // POST: api/trip
        /// <summary>
        /// Creates a new public trip.
        /// </summary>
        /// <param name="dto">Trip details to create.</param>
        /// <param name="createdById">ID of the user creating the trip.</param>
        /// <returns>The created trip data.</returns>
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTrip>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("Public/{createdById:int}")]
        public async Task<IActionResult> CreateTrip([FromBody] AddPublicTripDto dto, int createdById)
        {
            var result = await Sender.Send(new AddPublicTrip(dto, createdById));
            return Ok(result);
        }


        // DELETE: api/trip/{id}
        // POST: api/trip
        /// <summary>
        /// Delelte public trip.
        /// </summary>
        /// <param name="dto">Trip details to create.</param>
        /// <param name="createdById">ID of the user creating the trip.</param>
        /// <returns>The created trip data.</returns>
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpDelete("Public/{id:int}")]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            var result = await Sender.Send(new DeletePublicTrip(id));
            return Ok(result);
        }
        // POST: api/trip
        /// <summary>
        /// Creates a new Private trip.
        /// </summary>
        /// <param name="dto">Trip details to create.</param>
        /// <param name="createdById">ID of the user creating the trip.</param>
        /// <returns>The created trip data.</returns>
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTrip>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("Private/{createdById:int}")]
        public async Task<IActionResult> CreatePrivateTrip([FromBody] AddPrivateTripDto dto, int createdById)
        {
            var result = await Sender.Send(new AddPrivateTrip(dto, createdById));
            return Ok(result);
        }

        // DELETE: api/trip/{id}
        // POST: api/trip
        /// <summary>
        /// Delete  trip.
        /// </summary>
        /// <param name="dto">Trip details to create.</param>
        /// <param name="createdById">ID of the user creating the trip.</param>
        /// <returns>The created trip data.</returns>
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpDelete("Private/{id:int}")]
        public async Task<IActionResult> DeletePrivateTrip(int id)
        {
            var result = await Sender.Send(new DeletePrivateTrip(id));
            return Ok(result);
        }
    }
}
