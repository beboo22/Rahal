using ApplicationBusiness.Fetures.PaymentService;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymobController : ApiController
    {
        public PaymobController(ISender sender) : base(sender) { }

        /// <summary>
        /// Processes Paymob webhook notifications (Server-to-Server).
        /// </summary>
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] JsonElement payload)
        {
            var hmac = Request.Query["hmac"];
            var result = await Sender.Send(new HandlePaymobWebhookCommand(payload, hmac));

            return ProcessResult(result);
        }

        /// <summary>
        /// Client-side callback from Paymob after transaction attempt.
        /// </summary>
        [HttpGet("paymob-callback")]
        public async Task<IActionResult> Handlepaymobcallback([FromQuery] string success, [FromQuery] string merchant_order_id)
        {
            // Redirection logic remains standard
            if (bool.TryParse(success, out bool isSuccess) && isSuccess)
                return Redirect("https://rahhal-app.vercel.app/");

            return Redirect("https://rahhal-app.vercel.app/faild");
        }

        /// <summary>
        /// Initiates payment for a Public Trip.
        /// </summary>
        [HttpPost("payforpublicTrip")]
        public async Task<IActionResult> payforpublic(int id)
        {
            var result = await Sender.Send(new PublicTripCreatePayment(id));
            return ProcessResult(result);
        }

        /// <summary>
        /// Initiates payment for a Private Trip.
        /// </summary>
        [HttpPost("payforprivateTrip")]
        public async Task<IActionResult> payforPrivate(int id)
        {
            var result = await Sender.Send(new PrivateTripCreatePayment(id));
            return ProcessResult(result);
        }

        /// <summary>
        /// Initiates payment for a Flight booking.
        /// </summary>
        [HttpPost("payforFlight")]
        public async Task<IActionResult> payforFlight(int id)
        {
            var result = await Sender.Send(new FlightCreatePayment(id));
            return ProcessResult(result);
        }

        /// <summary>
        /// Initiates payment for a Hotel booking.
        /// </summary>
        [HttpPost("payforHotel")]
        public async Task<IActionResult> payforHotel(int id)
        {
            var result = await Sender.Send(new HotleCreatePayment(id));
            return ProcessResult(result);
        }
    }
}
