using ApplicationBusiness.Services;
using Domain.Entity.Identity;
using Domain.Entity.TripEntity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Features.PaymentService
{
    public class PaymobService : IPaymobService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymobService> _logger;

        private string SecretKey => _configuration["PaymobSettings:SecretKey"]
            ?? throw new InvalidOperationException("Paymob SecretKey not configured");

        private string PublicKey => _configuration["PaymobSettings:PublicKey"]
            ?? throw new InvalidOperationException("Paymob PublicKey not configured");
        // ✅ صحيح - Array of Integration IDs
        private int[] IntegrationIds =>
            _configuration.GetSection("PaymobSettings:IntegrationIds")
                .GetChildren()
                .Select(x => int.Parse(x.Value!))
                .ToArray();




        private int PayPalIntegrationId => int.Parse(
            _configuration["PaymobSettings:PayPalIntegrationId"]
            ?? throw new InvalidOperationException("Paymob PayPalIntegrationId not configured"));

        private string BaseUrl => "https://accept.paymob.com";

        public PaymobService(HttpClient httpClient, IConfiguration configuration, ILogger<PaymobService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> InitiatePaymentAsync(Order order, string? callbackUrl = null, string? returnUrl = null)
        {
            ArgumentNullException.ThrowIfNull(order);

            try
            {
                var result = await CreateIntentionAsync(
                    amountCents: (int)(order.TotalBookingPrice * 100),
                    merchantOrderId: $"{order.ProviderRef}",
                    itemName: "Order",
                    itemDescription: order.ItemDesc,
                    billingData: BuildBillingData(order.User),
                    callbackUrl,
                    returnUrl
                );

                return $"{BaseUrl}/unifiedcheckout/?publicKey={PublicKey}&clientSecret={result.ClientSecret}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Paymob payment initiation failed for OrderId {OrderId}", order.ProviderRef);
                throw;
            }
        }

        private async Task<PaymobIntentionResponse> CreateIntentionAsync(
            int amountCents,
            string merchantOrderId,
            string itemName,
            string itemDescription,
            object billingData,
            string? callbackUrl,
            string? returnUrl)
        {
            var requestBody = new
            {
                amount = amountCents,
                currency = "EGP",
                payment_methods = IntegrationIds, // PayPal USD only
                //payment_methods = new[] { "paypal" }, // PayPal USD only
                items = new[]
                {
                new
                {
                    name = itemName,
                    amount = amountCents,
                    description = itemDescription,
                    quantity = 1
                }
            },
                billing_data = billingData,
                special_reference = merchantOrderId,
                notification_url = callbackUrl ?? _configuration["PaymobSettings:CallbackUrl"],
                redirection_url = returnUrl ?? _configuration["PaymobSettings:ReturnUrl"]
            };

            string json = JsonSerializer.Serialize(requestBody);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/v1/intention/")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Token", SecretKey);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Paymob intention response: {StatusCode} | {Body}",
                response.StatusCode, content);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(
                    $"Paymob API error: {response.StatusCode}, Message: {content}");

            return JsonSerializer.Deserialize<PaymobIntentionResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        private static object BuildBillingData(User? customer)
        {
            string firstName = customer?.FName ?? "Customer";
            string lastName = customer?.LName ?? "NA";

            return new
            {
                apartment = "NA",
                first_name = firstName,
                last_name = lastName,
                street = "NA",
                building = "NA",
                phone_number = NormalizeEgyptPhone(""),
                city = "Cairo",
                country = "EG",
                email = customer?.Email ?? "customer@example.com",
                floor = "NA",
                state = "Cairo"
            };
        }

        private static string NormalizeEgyptPhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "+201000000000";

            phone = phone.Trim();

            if (phone.StartsWith("+20"))
                return phone;

            if (phone.StartsWith("0"))
                return "+2" + phone;

            return "+20" + phone;
        }
    }
    public interface IPaymobService
    {
        Task<string> InitiatePaymentAsync(Order order, string? callbackUrl = null, string? returnUrl = null);
    }

    //public class PaymobIntentionResponse
    //{
    //    public string ClientSecret { get; set; } = string.Empty;
    //    public int IntentionOrderId { get; set; }
    //    public string Id { get; set; } = string.Empty;
    //}

    public class PaymobIntentionResponse
    {
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("intention_order_id")]
        public long IntentionOrderId { get; set; }

        [JsonPropertyName("payment_keys")]
        public List<PaymentKey>? PaymentKeys { get; set; }
    }

    public class PaymentKey
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("integration")]
        public long Integration { get; set; }
    }


}
