using Application.Abstraction.message;
using Application.Abstraction.spacification;
using Application.Features.PaymentService;
using ApplicationBusiness.Fetures.PaymentService.Strategies;
using ApplicationBusiness.Services;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TripEntity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ApplicationBusiness.Fetures.PaymentService
{
    public class HandlePaymobWebhookHandler : IRequestHandler<HandlePaymobWebhookCommand, bool>
    {
        private readonly IConfiguration _configuration;


        private readonly IReadGenericRepo<PaymentRequest> _paymentRepo;
        private readonly PaymentHandlerFactory _factory;
        private readonly ILogger<HandlePaymobWebhookHandler> _logger;

        public HandlePaymobWebhookHandler(
            IConfiguration configuration,
            ILogger<HandlePaymobWebhookHandler> logger,
            IReadGenericRepo<PaymentRequest> paymentRepo,
            PaymentHandlerFactory factory)
        {
            _configuration = configuration;
            _logger = logger;
            _paymentRepo = paymentRepo;
            _factory = factory;
        }

        public async Task<bool> Handle(HandlePaymobWebhookCommand request, CancellationToken cancellationToken)
        {
            var payload = request.Payload;

            try
            {
                _logger.LogInformation("Paymob Webhook Received: {Payload}", payload.ToString());

                // 🔥 1. تحقق من HMAC
                if (!IsValidHmac(request.Payload,request.hmac))
                {
                    _logger.LogWarning("Invalid Paymob HMAC");
                    return false;
                }
                var obj = payload.GetProperty("obj");

                bool success = obj.GetProperty("success").GetBoolean();

                if(!success)
                    return false;

                string providerRef = obj
                    .GetProperty("order")
                    .GetProperty("merchant_order_id")
                    .GetString()!;

                // 🔥 lookup

                var spec = new PaymentSpecification(providerRef);


                var payment = await _paymentRepo.GetByIDSpec(spec);

                if (payment == null)
                {
                    _logger.LogWarning("Payment not found");
                    return false;
                }

                // 🔥 dispatch
                var handler = _factory.GetHandler(payment.EntityType);

                await handler.HandleAsync(payment.EntityId, success);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook error");
                throw;
            }



        }

        // ============================
        // 🔐 HMAC VALIDATION
        // ============================
        private bool IsValidHmac(JsonElement payload, string hmacFromRequest)
        {
            var hmacSecret = _configuration["PaymobSettings:HmacSecret"];
            if (string.IsNullOrEmpty(hmacSecret))
                throw new InvalidOperationException("HmacSecret not configured");

            if (string.IsNullOrEmpty(hmacFromRequest))
                return false;

            var obj = payload.GetProperty("obj");

            string Get(JsonElement element, string prop)
                => element.TryGetProperty(prop, out var val) ? val.ToString() : "";

            string GetNested(JsonElement element, string parent, string child)
                => element.GetProperty(parent).TryGetProperty(child, out var val) ? val.ToString() : "";

            var concatenated =
                Get(obj, "amount_cents") +
                Get(obj, "created_at") +
                Get(obj, "currency") +
                Get(obj, "error_occured").ToLower() +
                Get(obj, "has_parent_transaction").ToLower() +
                Get(obj, "id") +
                Get(obj, "integration_id") +
                Get(obj, "is_3d_secure").ToLower() +
                Get(obj, "is_auth").ToLower() +
                Get(obj, "is_capture").ToLower() +
                Get(obj, "is_refunded").ToLower() +
                Get(obj, "is_standalone_payment").ToLower() +
                Get(obj, "is_voided").ToLower() +
                Get(obj.GetProperty("order"), "id") +
                Get(obj, "owner") +
                Get(obj, "pending").ToLower() +
                GetNested(obj, "source_data", "pan") +
                GetNested(obj, "source_data", "sub_type") +
                GetNested(obj, "source_data", "type") +
                Get(obj, "success").ToLower();

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hmacSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
            var calculatedHmac = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return calculatedHmac == hmacFromRequest;
        }

        private static int ExtractOrderId(string merchantOrderId)
        {
            // ORD/{Guid}/{OrderId}
            var parts = merchantOrderId.Split('/');
            return int.Parse(parts[^1]);
        }
    }
    internal class PaymentCommandHandler : ICommandHandler<CreatePayment, ApiResponse>
    {
        private IWriteGenericRepo<PaymentRequest> _Wrepo;
        private IReadGenericRepo<BookingPublicTrip> _ROrepo;
        private IWriteUnitOfWork _unitOfWork;

        private IPaymobService paymobService;

        public PaymentCommandHandler(IPaymobService paymobService, IReadGenericRepo<BookingPublicTrip> rOrepo, IWriteGenericRepo<PaymentRequest> wrepo, IWriteUnitOfWork unitOfWork)
        {
            this.paymobService = paymobService;
            _ROrepo = rOrepo;
            _Wrepo = wrepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse> Handle(CreatePayment request, CancellationToken cancellationToken)
        {


            var order = _ROrepo.GetAll().Where(x => x.Id == request.BookId && x.IsPaid == false && x.Canceled == false).Select(x => new Order
            {
                User = new User
                {
                    Email = x.User.Email,

                    phoneNumbers = x.User.phoneNumbers,
                    FName = x.User.FName,

                },
                TotalBookingPrice = x.TotalBookingPrice,
            }).FirstOrDefault();


            //var order = ;
            if (order == null)
                return new ApiResponse(404);

            order.ProviderRef = Guid.NewGuid().ToString();

            var url = await paymobService.InitiatePaymentAsync(order);

            if (url == null)
                return new ApiResponse(500);

            await _unitOfWork.BeginTransactionAsync();

            await _Wrepo.AddAsync(new PaymentRequest
            {
                ProviderRef = order.ProviderRef,
                EntityId = request.BookId,
                EntityType = PaymentEntityType.PublicTrip,
                IsPaid = false
            });

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();


            return new ApiResultResponse<string>(200, url);




        }
    }
}
