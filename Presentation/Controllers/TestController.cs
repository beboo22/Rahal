using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ApiController
    {
        public TestController(ISender sender) : base(sender) { }

    //    [HttpPost("flight-availability")]
    //    public async Task<IActionResult> FlightAvailability(
    //FlightAvailabilityRequest request)
    //    {
    //        var result = await Sender.Send(
    //            new GetFlightAvailabilityQuery(request));

    //        return Ok(result);
    //    }




    //    [HttpPost("search")]
    //    public async Task<IActionResult> SearchHotels(
    //[FromBody] HotelSearchRequest request)
    //    {
    //        var response = await Sender
    //            .Send(new SearchHotelsQuery(request));

    //        return StatusCode(response.statusCode, response);
    //    }



    }
}
