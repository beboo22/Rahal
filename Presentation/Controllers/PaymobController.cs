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

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] JsonElement payload)
        {
            var hmac = Request.Query["hmac"];
            var result = await Sender.Send(new HandlePaymobWebhookCommand(payload, hmac));
            return Ok(result);
        }
        [HttpGet("paymob-callback")]
        public async Task<IActionResult> Handlepaymobcallback([FromQuery] string success, [FromQuery] string merchant_order_id)
        {
            if (bool.Parse(success))
                return Redirect("https://rahhal-app.vercel.app/");
            return Redirect("https://rahhal-app.vercel.app/faild");
        }
        [HttpPost("payforpublic")]
        public async Task<IActionResult> payforpublic()
        {
            var result = await Sender.Send(new CreatePayment(1));
            return Ok(result);
        }



    }
}
