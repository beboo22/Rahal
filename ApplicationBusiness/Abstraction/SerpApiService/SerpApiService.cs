using ApplicationBusiness.Configuration;
using ApplicationBusiness.Dtos.Flights;
using ApplicationBusiness.Dtos.Hotels;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.BaseResponce;

namespace ApplicationBusiness.Abstraction.SerpApiService
{
    public sealed class SerpApiService : ISerpApiService
    {
        private readonly HttpClient _httpClient;
        private readonly SerpApiSettings _settings;
        private readonly ILogger<SerpApiService> _logger;
        private readonly IDistributedCache? _cache;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        public SerpApiService(
            HttpClient httpClient,
            IOptions<SerpApiSettings> settings,
            ILogger<SerpApiService> logger,
            IDistributedCache? cache = null)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
            _cache = cache;
        }

        // ─────────────────────────── FLIGHTS ────────────────────────────

        public async Task<ApiResponse> SearchFlightsAsync(
            FlightSearchRequest request,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = CacheKeys.Flights(request);

            if (_settings.EnableCaching && _cache is not null)
            {
                var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (cached is not null)
                {
                    _logger.LogDebug("Flight cache hit: {Key}", cacheKey);
                    var cachedResult = JsonSerializer.Deserialize<FlightSearchResponse>(cached, _jsonOptions);
                    return new ApiResultResponse<FlightSearchResponse>(200,cachedResult!, "Flights retrieved from cache.");
                }
            }

            var queryParams = BuildFlightQuery(request);
            var url = BuildUrl(queryParams);

            _logger.LogInformation("Calling SerpAPI Flights: {Url}", MaskApiKey(url));

            try
            {
                var httpResponse = await _httpClient.GetAsync(url, cancellationToken);
                return await HandleFlightResponse(httpResponse, cacheKey, cancellationToken);
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("SerpAPI Flights request timed out.");
                return new ApiResponse(500,"Request timed out. Please try again.TIMEOUT");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling SerpAPI Flights.");
                return new ApiResponse(500,"Unable to reach search provider.HTTP_ERROR");
            }
        }

        private async Task<ApiResponse> HandleFlightResponse(
            HttpResponseMessage httpResponse,
            string cacheKey,
            CancellationToken cancellationToken)
        {
            var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (httpResponse.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogWarning("SerpAPI rate limit hit.");
                return new ApiResponse((int)httpResponse.StatusCode, "Search rate limit reached. Please wait and retry.RATE_LIMIT");
            }

            if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogError("SerpAPI invalid API key.");
                return new ApiResponse((int)httpResponse.StatusCode, "Invalid API key configuration.INVALID_KEY");
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogError("SerpAPI Flights returned {StatusCode}: {Content}", httpResponse.StatusCode, content);
                return new ApiResponse((int)httpResponse.StatusCode, $"Search provider error: {httpResponse.StatusCode},PROVIDER_ERROR");
            }

            var json = JsonNode.Parse(content);
            if (json is null)
                return new ApiResponse((int)httpResponse.StatusCode, "Empty response from search provider.EMPTY_RESPONSE");

            // Check for SerpAPI error field
            var errorMsg = json["error"]?.GetValue<string>();
            if (!string.IsNullOrEmpty(errorMsg))
            {
                _logger.LogError("SerpAPI returned error: {Error}", errorMsg);
                return new ApiResponse((int)httpResponse.StatusCode, $"{errorMsg} SERP_ERROR");
            }
            try
            {
                Console.WriteLine(json);
            var response = new FlightSearchResponse
            {
                SearchId = json["search_metadata"]?["id"]?.GetValue<string>() ?? string.Empty,
                currency = json["search_parameters"]?["currency"]?.GetValue<string>() ?? string.Empty,
                BestFlights = ParseFlights(json["best_flights"]),
                OtherFlights = ParseFlights(json["other_flights"]),
                PriceInsights = ParsePriceInsights(json["price_insights"])
            };

            if (!response.BestFlights.Any() && !response.OtherFlights.Any())
            {
                _logger.LogInformation("No flight results found.");
                return new ApiResultResponse<FlightSearchResponse>(200,response, "No flights found for the given criteria.");
            }

            if (_settings.EnableCaching && _cache is not null)
            {
                var serialized = JsonSerializer.Serialize(response, _jsonOptions);
                await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.CacheDurationMinutes)
                }, cancellationToken);
            }

            return new ApiResultResponse<FlightSearchResponse>(200,response, "Flights retrieved successfully.");
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(json);

                throw ex;
            }
        }

        private static List<FlightResult> ParseFlights(JsonNode? node)
        {
            var list = new List<FlightResult>();
            if (node is not JsonArray arr) return list;

            foreach (var item in arr)
            {
                if (item is null) continue;
                var result = new FlightResult
                {
                    Price = item["price"]?.GetValue<int>() ?? 0,
                    TotalDuration = item["total_duration"]?.GetValue<int>() ?? 0,
                    Type = item["type"]?.GetValue<string>() ?? string.Empty,
                    BookingToken = item["booking_token"]?.GetValue<string>() ?? string.Empty,
                    Flights = ParseFlightLegs(item["flights"])
                };
                list.Add(result);
            }
            return list;
        }

        private static List<FlightLeg> ParseFlightLegs(JsonNode? node)
        {
            var list = new List<FlightLeg>();
            if (node is not JsonArray arr) return list;

            foreach (var item in arr)
            {
                if (item is null) continue;
                list.Add(new FlightLeg
                {
                    DepartureAirport = new Airport
                    {
                        Name = item["departure_airport"]?["name"]?.GetValue<string>() ?? string.Empty,
                        Id = item["departure_airport"]?["id"]?.GetValue<string>() ?? string.Empty,
                        Time = item["departure_airport"]?["time"]?.GetValue<string>() ?? string.Empty
                    },
                    ArrivalAirport = new Airport
                    {
                        Name = item["arrival_airport"]?["name"]?.GetValue<string>() ?? string.Empty,
                        Id = item["arrival_airport"]?["id"]?.GetValue<string>() ?? string.Empty,
                        Time = item["arrival_airport"]?["time"]?.GetValue<string>() ?? string.Empty
                    },
                    Duration = item["duration"]?.GetValue<int>() ?? 0,
                    Airplane = item["airplane"]?.GetValue<string>() ?? string.Empty,
                    Airline = item["airline"]?.GetValue<string>() ?? string.Empty,
                    AirlineLogo = item["airline_logo"]?.GetValue<string>() ?? string.Empty,
                    TravelClass = item["travel_class"]?.GetValue<string>() ?? string.Empty,
                    FlightNumber = item["flight_number"]?.GetValue<string>() ?? string.Empty,
                    Overnight = item["overnight"]?.GetValue<bool>() ?? false,
                    LegRoom = ParseLegroom(item["legroom"]?.GetValue<string>())
                });
            }
            return list;
        }
        private static int? ParseLegroom(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            // value is like "32 in" — extract the leading number
            var numberPart = value.Split(' ')[0];
            return int.TryParse(numberPart, out var result) ? result : null;
        }

        private static PriceInsights? ParsePriceInsights(JsonNode? node)
        {
            if (node is null) return null;
            return new PriceInsights
            {
                LowestPrice = node["lowest_price"]?.GetValue<int>() ?? 0,
                PriceLevel = node["price_level"]?.GetValue<string>() ?? string.Empty
            };
        }

        // ─────────────────────────── HOTELS ─────────────────────────────

        public async Task<ApiResponse> SearchHotelsAsync(
            HotelSearchRequest request,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = CacheKeys.Hotels(request);

            if (_settings.EnableCaching && _cache is not null)
            {
                var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (cached is not null)
                {
                    _logger.LogDebug("Hotel cache hit: {Key}", cacheKey);
                    var cachedResult = JsonSerializer.Deserialize<HotelSearchResponse>(cached, _jsonOptions);
                    return new ApiResultResponse<HotelSearchResponse>(200, cachedResult!, "Hotels retrieved from cache.");
                }
            }

            var queryParams = BuildHotelQuery(request);
            var url = BuildUrl(queryParams);

            _logger.LogInformation("Calling SerpAPI Hotels: {Url}", MaskApiKey(url));

            try
            {
                var httpResponse = await _httpClient.GetAsync(url, cancellationToken);
                return await HandleHotelResponse(httpResponse, cacheKey, cancellationToken);
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("SerpAPI Hotels request timed out.");
                return new ApiResponse(500,"Request timed out. Please try again.TIMEOUT");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling SerpAPI Hotels.");
                return new ApiResponse(500,"Unable to reach search provider.HTTP_ERROR");
            }
        }

        private async Task<ApiResponse> HandleHotelResponse(
            HttpResponseMessage httpResponse,
            string cacheKey,
            CancellationToken cancellationToken)
        {
            var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (httpResponse.StatusCode == HttpStatusCode.TooManyRequests)
                return new ApiResponse((int)httpResponse.StatusCode,"Search rate limit reached.RATE_LIMIT");

            if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                return new ApiResponse((int)httpResponse.StatusCode,"Invalid API key configuration.INVALID_KEY");

            if (!httpResponse.IsSuccessStatusCode)
                return new ApiResponse((int)httpResponse.StatusCode, $"Search provider error: {httpResponse.StatusCode},PROVIDER_ERROR");

            var json = JsonNode.Parse(content);
            if (json is null)
                return new ApiResponse(500,"Empty response from search provider.EMPTY_RESPONSE");

            var errorMsg = json["error"]?.GetValue<string>();
            if (!string.IsNullOrEmpty(errorMsg))
            {
                _logger.LogError("SerpAPI returned error: {Error}", errorMsg);
                return new ApiResponse((int)httpResponse.StatusCode, $"{errorMsg} SERP_ERROR");
            }
            var properties =
    json["properties"] ??
    json["non_matching_properties"];
            var response = new HotelSearchResponse
            {
                currency = json["search_parameters"]?["currency"].GetValue<string>()??string.Empty,
                SearchId = json["search_metadata"]?["id"]?.GetValue<string>() ?? string.Empty,
                Properties = ParseHotels(properties)
            };

            if (!response.Properties.Any())
            {
                _logger.LogInformation("No hotel results found.");
                return new ApiResultResponse<HotelSearchResponse>(200,response, "No hotels found for the given criteria.");
            }

            if (_settings.EnableCaching && _cache is not null)
            {
                var serialized = JsonSerializer.Serialize(response, _jsonOptions);
                await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.CacheDurationMinutes)
                }, cancellationToken);
            }

            return new ApiResultResponse<HotelSearchResponse>(200,response, "Hotels retrieved successfully.");
        }

        private static List<HotelResult> ParseHotels(JsonNode? node)
        {
            var list = new List<HotelResult>();
            if (node is not JsonArray arr) return list;

            foreach (var item in arr)
            {
                if (item is null) continue;
                var hotel = new HotelResult
                {
                    Name = item["name"]?.GetValue<string>() ?? string.Empty,
                    Description = item["description"]?.GetValue<string>() ?? string.Empty,
                    Link = item["link"]?.GetValue<string>() ?? string.Empty,
                    Rating = item["overall_rating"]?.GetValue<double>() ?? 0,
                    Reviews = item["reviews"]?.GetValue<int>() ?? 0,
                    PropertyToken = item["property_token"]?.GetValue<string>() ?? string.Empty,
                    SponsoredHotel = item["sponsored"]?.GetValue<bool>() ?? false,
                    Location = new HotelLocation
                    {
                        Latitude = item["gps_coordinates"]?["latitude"]?.GetValue<double>() ?? 0,
                        Longitude = item["gps_coordinates"]?["longitude"]?.GetValue<double>() ?? 0
                    },
                    LowestPrice = item["rate_per_night"]?["lowest"]?
                        .GetValue<string>()
                        ?.Replace("$", "")
                        ?.Replace(",", "")
                        .Let(s => decimal.TryParse(s, out var p) ? p : 0) ?? 0
                };

                // Images
                if (item["images"] is JsonArray imgs)
                    foreach (var img in imgs)
                        hotel.Images.Add(img?["thumbnail"]?.GetValue<string>() ?? string.Empty);

                // Amenities
                if (item["amenities"] is JsonArray amenities)
                    foreach (var a in amenities)
                    {
                        var amenityName = a?.GetValue<string>();
                        if (!string.IsNullOrEmpty(amenityName))
                            hotel.Amenities.Add(amenityName);
                    }

                list.Add(hotel);
            }
            return list;
        }

        // ─────────────────────────── HELPERS ────────────────────────────

        private Dictionary<string, string> BuildFlightQuery(FlightSearchRequest request)
        {
            var p = new Dictionary<string, string>
            {
                ["engine"] = "google_flights",
                ["api_key"] = _settings.ApiKey,
                ["departure_id"] = request.DepartureId.ToUpperInvariant(),
                ["arrival_id"] = request.ArrivalId.ToUpperInvariant(),
                ["outbound_date"] = request.OutboundDate,
                ["adults"] = request.Adults.ToString(),
                ["children"] = request.Children.ToString(),
                ["travel_class"] = ((int)request.TravelClass).ToString(),
                ["type"] = ((int)request.TripType).ToString(),
                ["currency"] = request.Currency.ToUpperInvariant(),
                ["gl"] = request.Gl.ToLowerInvariant(),
                ["hl"] = request.Hl.ToLowerInvariant()
            };

            if (!string.IsNullOrEmpty(request.ReturnDate))
                p["return_date"] = request.ReturnDate;
            if (request.ChildrenAges.Any())
                p["children_ages"] = string.Join(",", request.ChildrenAges);
            return p;
        }

        private Dictionary<string, string> BuildHotelQuery(HotelSearchRequest request)
        {
            var p = new Dictionary<string, string>
            {
                ["engine"] = "google_hotels",
                ["api_key"] = _settings.ApiKey,
                ["q"] = request.Destination,
                ["check_in_date"] = request.CheckInDate,
                ["check_out_date"] = request.CheckOutDate,
                ["adults"] = request.Adults.ToString(),
                ["children"] = request.Children.ToString(),
                //["children_ages"] = request.children_ages.ToString(),
                ["rooms"] = request.Rooms.ToString(),
                ["currency"] = request.Currency.ToUpperInvariant(),
                //["sort_by"] = ((int)request.SortBy).ToString(),
                ["gl"] = request.Gl.ToLowerInvariant(),
                ["hl"] = request.Hl.ToLowerInvariant()
            };

            //if (request.MinRating.HasValue)
                //p["rating"] = request.MinRating.Value.ToString();

            if (request.MinPrice.HasValue)
                p["min_price"] = request.MinPrice.Value.ToString("F0");

            if (request.MaxPrice.HasValue)
                p["max_price"] = request.MaxPrice.Value.ToString("F0");

            if (request.Amenities.Any())
                p["amenities"] = string.Join(",", request.Amenities);
            if (request.ChildrenAges.Any())
                p["children_ages"] = string.Join(",", request.ChildrenAges);

            return p;
        }

        private string BuildUrl(Dictionary<string, string> parameters)
        {
            var query = string.Join("&", parameters.Select(kv =>
                $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
            return $"{_settings.BaseUrl}?{query}";
        }

        private static string MaskApiKey(string url) =>
            System.Text.RegularExpressions.Regex.Replace(url, @"api_key=[^&]+", "api_key=***");
    }

    // Extension method for fluent null handling
    internal static class ObjectExtensions
    {
        public static TResult Let<T, TResult>(this T obj, Func<T, TResult> transform) => transform(obj);
    }

    public static class CacheKeys
    {
        public static string Flights(FlightSearchRequest r) =>
            $"flights:{r.DepartureId}:{r.ArrivalId}:{r.OutboundDate}:{r.ReturnDate}:{r.Adults}:{r.Children}:{r.TravelClass}:{r.TripType}:{r.Currency}";

        public static string Hotels(HotelSearchRequest r) =>
            $"hotels:{Encode(r.Destination)}:{r.CheckInDate}:{r.CheckOutDate}:{r.Adults}:{r.Rooms}:{r.Currency}:{r.MinPrice}:{r.MaxPrice}";

        private static string Encode(string s) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(s)).Replace("=", "");
    }
}
