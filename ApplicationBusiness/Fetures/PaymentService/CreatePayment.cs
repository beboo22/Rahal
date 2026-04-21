using Application.Abstraction.message;
using Domain.BaseResponce;
using MediatR;
using System.Text.Json;
using System.Windows.Input;

namespace ApplicationBusiness.Fetures.PaymentService
{
    public record HandlePaymobWebhookCommand(JsonElement Payload,string hmac) : IRequest<bool>;
    public record CreatePayment(int BookId):ICommand<ApiResponse>;


//public class HandlePaymobWebhookCommand : ICommand<bool>
//    {
//        public JsonElement Payload { get; set; }

//        public HandlePaymobWebhookCommand(JsonElement payload)
//        {
//            Payload = payload;
//        }
//    }
}