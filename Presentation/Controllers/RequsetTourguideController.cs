using MediatR;
using Microsoft.AspNetCore.Mvc;
using ApplicationBusiness.Fetures.RequestTourGuideForTrip.Command;
using Domain.BaseResponce;
using Microsoft.AspNetCore.Http;
using ApplicationBusiness.Fetures.RequestTourGuideForTrip.Query;
using ApplicationBusiness.Fetures.RequestTourGuideForTrip.Query.Response;
using ApplicationBusiness.Fetures.Profile.Command;
using ApplicationBusiness.Fetures.Profile.Query;

namespace Presentation.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RequsetTourguideController : ApiController
    {


        public RequsetTourguideController(ISender sender) : base(sender)
        {
        }



        /// <summary>
        /// Request tour guides for a private trip.
        /// </summary>
        [HttpPost("trips/private/request-tour-guide")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RequestPrivateTripTourGuide(
            [FromBody] RequestTourGuidePrivateTripCommand command,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(command, cancellationToken);

            // If your base ProcessResult handles ApiResponse, use it here. 
            // Otherwise, you can handle it explicitly based on the status code:
            return StatusCode(result.statusCode, result);
        }
        /// <summary>
        /// Request tour guides for a private trip.
        /// </summary>
        [HttpPut("trips/private/Accept-request-tour-guide")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AcceptRequestPrivateTripTourGuide(
            [FromBody] AcceptPivRequest command,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(command, cancellationToken);

            // If your base ProcessResult handles ApiResponse, use it here. 
            // Otherwise, you can handle it explicitly based on the status code:
            return StatusCode(result.statusCode, result);
        }

        /// <summary>
        /// Request tour guides for a public trip.
        /// </summary>
        [HttpPost("trips/public/request-tour-guide")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RequestPublicTripTourGuide(
            [FromBody] RequestTourGuidePubTripCommand command,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(command, cancellationToken);
            return StatusCode(result.statusCode, result);
        }
        /// <summary>
        /// Request tour guides for a public trip.
        /// </summary>
        [HttpPut("trips/public/Accept-request-tour-guide")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AcceptRequestPublicTripTourGuide(
            [FromBody] AcceptPubRequest command,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(command, cancellationToken);
            return StatusCode(result.statusCode, result);
        }


        /// <summary>
        /// Get all pending private and public trip requests for a specific tour guide.
        /// </summary>
        /// <param name="tourGuideId">The unique identifier of the tour guide</param>
        /// <param name="cancellationToken"></param>
        [HttpGet("tour-guides/requests")]
        [ProducesResponseType(typeof(ApiResultResponse<TemplateRequestTourGuide>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRequestsForTourGuide(
            CancellationToken cancellationToken)
        {

            var userid = GetUserId();
            if (userid == null)
                return Unauthorized();
            // 1. Send the query record containing the route parameter ID
            var result = await Sender.Send(new GetRequsetForTougiude(userid.Value), cancellationToken);

            // 2. Pass it off to your base class processing method
            return ProcessResult(result);
        }



        /// <summary>
        /// جلب قائمة المرشدين السياحيين المتواجدين في دولة معينة
        /// </summary>
        /// <param name="country">اسم الدولة المراد البحث فيها</param>
        /// <param name="cancellationToken"></param>
        [HttpGet("tour-guides/search")]
        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateTourSearch>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTourGuidesByCountry(
            [FromQuery] string country,
            CancellationToken cancellationToken)
        {
            // 1. إرسال الـ Query وبداخلها اسم الدولة القادم من الـ URL
            var result = await Sender.Send(new GetTourgideInSpecCountry(country), cancellationToken);

            // 2. معالجة النتيجة بواسطة الـ Base ApiController الخاص بك
            return ProcessResult(result);
        }



    }
}
