using ApplicationBusiness.Dtos.Flights;
using ApplicationBusiness.Dtos.Hotels;
using ApplicationBusiness.Dtos.Photos;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Runtime;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ApplicationBusiness.Configuration;
using StackExchange.Redis;
using System.Text.Json;
using Domain.Entity.photo;
using Domain.Abstraction;
using ApplicationBusiness.Abstraction.spacification;
using Microsoft.Extensions.Options;

namespace ApplicationBusiness.Abstraction.SerpApiService
{
    public class SerpPhotoApiService : ISerpPhotoApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true


        };

        IReadGenericRepo<PhotoSearchResponse> _readGenericRepo;

        private readonly ILogger<SerpPhotoApiService> _logger;
        private readonly SerpApiSettings _settings;
        private readonly IDatabase _cache; // StackExchange.Redis
        public SerpPhotoApiService(HttpClient httpClient, ILogger<SerpPhotoApiService> logger, IOptions<SerpApiSettings> settings, IConnectionMultiplexer redis, IReadGenericRepo<PhotoSearchResponse> readGenericRepo)
        {
            _httpClient = httpClient;
            _logger = logger;
            _settings = settings.Value;
            _cache = redis.GetDatabase();
            _readGenericRepo = readGenericRepo;
        }

        public async Task<ApiResponse> SearchPhotoAsync(SearchPhotoReq request, CancellationToken cancellationToken = default)
        {
            var exactKey = CacheKeys.PhotoExact(request);
            var groupKey = CacheKeys.PhotoGroup(request);
            // 1. Check Cache


            if (_settings.EnableCaching && _cache is not null)
            {
                // 1. exact redis match
                //var exactCached = await _cache.GetStringAsync(exactKey, cancellationToken);
                var exactCached = await _cache.StringGetAsync(exactKey);
                if (!string.IsNullOrWhiteSpace(exactCached))
                {
                    _logger.LogDebug("Exact Photo cache hit: {Key}", exactKey);

                    var result = JsonSerializer.Deserialize<PhotoSearchResponse>(exactCached!, _jsonOptions);

                    return new ApiResultResponse<PhotoSearchResponse>(
                        224,
                        result!,
                        "Photos retrieved from exact cache.");
                }

                // 2. grouped redis match
                //var groupCached = await _cache.GetStringAsync(groupKey, cancellationToken);
                var groupCached = await _cache.StringGetAsync(groupKey);
                if (!string.IsNullOrWhiteSpace(groupCached))
                {
                    _logger.LogDebug("Grouped Photo cache hit: {Key}", groupKey);

                    var result = JsonSerializer.Deserialize<PhotoSearchResponse>(groupCached!, _jsonOptions);

                    return new ApiResultResponse<PhotoSearchResponse>(
                        224,
                        result!,
                        "Photo retrieved from similar cached search.");
                }
            }



            var photo = await _readGenericRepo.GetByIDSpec(new PhotoSpec(new PhotoFilter { SearchId = CacheKeys.PhotoExactOrgin(request), PageIndex = request.PageIndex, PageSize = request.PageSize }));

            if (photo != null)
                return new ApiResultResponse<PhotoSearchResponse>(200, photo, "Photo retrieved from similar DB search.");

            // 2. Build Query & Call API
            var queryParams = BuildPhotoQuery(request);
            var url = BuildUrl(queryParams);

            try
            {
                var httpResponse = await _httpClient.GetAsync(url, cancellationToken);
                return await HandlePhotoResponse(httpResponse, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during SerpAPI Photo Search");
                return new ApiResponse(500, $"Internal search error. {ex}");
            }
        }

        private async Task<ApiResponse> HandlePhotoResponse(HttpResponseMessage httpResponse, CancellationToken cancellationToken)
        {
            var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            if (!httpResponse.IsSuccessStatusCode) return new ApiResponse((int)httpResponse.StatusCode, "SerpApi Error");

            var json = JsonNode.Parse(content);
            if (json == null) return new ApiResponse(500, "Empty Response");

            // Map JSON to Domain Model
            var response = new PhotoSearchResponse
            {
                SearchId = json["search_metadata"]?["id"]?.GetValue<string>() ?? string.Empty,
                Images = json["images_results"]?.AsArray().Select(item => new PhotoResultItem
                {
                    Position = item?["position"]?.GetValue<int>() ?? 0,
                    Title = item?["title"]?.GetValue<string>() ?? string.Empty,
                    Source = item?["source"]?.GetValue<string>() ?? string.Empty,
                    Thumbnail = item?["thumbnail"]?.GetValue<string>() ?? string.Empty,
                    Original = item?["original"]?.GetValue<string>() ?? string.Empty,
                    Link = item?["link"]?.GetValue<string>() ?? string.Empty,
                    OriginalWidth = item?["original_width"]?.GetValue<int>() ?? 0,
                    OriginalHeight = item?["original_height"]?.GetValue<int>() ?? 0
                }).ToList() ?? new List<PhotoResultItem>()
            };

            // 3. Save to Cache
            //if (_settings.EnableCaching && _cache is not null && response.Images.Any())
            //{
            //    var serialized = JsonSerializer.Serialize(response);
            //    await _cache.StringSetAsync(cacheKey, serialized, TimeSpan.FromMinutes(_settings.CacheDurationMinutes));
            //}

            return new ApiResultResponse<PhotoSearchResponse>(200, response, "Success");
        }

        private Dictionary<string, string> BuildPhotoQuery(SearchPhotoReq request)
        {
            return new Dictionary<string, string>
            {
                ["engine"] = "google_images",
                ["q"] = request.Title,
                ["location"] = request.location,
                ["google_domain"] = "google.com",
                ["hl"] = "en",
                ["gl"] = "us",
                ["api_key"] = _settings.ApiKey
            };
        }

        private string BuildUrl(Dictionary<string, string> parameters)
        {
            var query = string.Join("&", parameters.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
            return $"{_settings.BaseUrl}?{query}";
        }
    }
    //internal class SerpPhotoApiService : ISerpPhotoApiService
    //{


    //    public Task<ApiResponse> SearchPhotoAsync(SearchPhotoReq request, CancellationToken cancellationToken = default)
    //    {
    //        var exactKey = CacheKeys.FlightsExact(request);
    //        var groupKey = CacheKeys.FlightsGroup(request);

    //        if (_settings.EnableCaching && _cache is not null)
    //        {
    //            // 1. exact redis match
    //            //var exactCached = await _cache.GetStringAsync(exactKey, cancellationToken);
    //            var exactCached = await _cache.StringGetAsync(exactKey);
    //            if (!string.IsNullOrWhiteSpace(exactCached))
    //            {
    //                _logger.LogDebug("Exact flight cache hit: {Key}", exactKey);

    //                var result = JsonSerializer.Deserialize<FlightSearchHistory>(exactCached, _jsonOptions);

    //                return new ApiResultResponse<FlightSearchHistory>(
    //                    224,
    //                    result!,
    //                    "Flights retrieved from exact cache.");
    //            }

    //            // 2. grouped redis match
    //            //var groupCached = await _cache.GetStringAsync(groupKey, cancellationToken);
    //            var groupCached = await _cache.StringGetAsync(groupKey);
    //            if (!string.IsNullOrWhiteSpace(groupCached))
    //            {
    //                _logger.LogDebug("Grouped flight cache hit: {Key}", groupKey);

    //                var result = JsonSerializer.Deserialize<FlightSearchHistory>(groupCached, _jsonOptions);

    //                return new ApiResultResponse<FlightSearchHistory>(
    //                    225,
    //                    result!,
    //                    "Flights retrieved from similar cached search.");
    //            }
    //        }
    //        var queryParams = BuildFlightQuery(request);
    //        var url = BuildUrl(queryParams);

    //        _logger.LogInformation("Calling SerpAPI Flights: {Url}", MaskApiKey(url));

    //        try
    //        {
    //            var httpResponse = await _httpClient.GetAsync(url, cancellationToken);
    //            //return await HandleFlightResponse(httpResponse, cacheKey, cancellationToken);
    //            return await HandleFlightResponse(httpResponse, cancellationToken);
    //        }
    //        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
    //        {
    //            _logger.LogWarning("SerpAPI Flights request timed out.");
    //            return new ApiResponse(500, "Request timed out. Please try again.TIMEOUT");
    //        }
    //        catch (HttpRequestException ex)
    //        {
    //            _logger.LogError(ex, "HTTP error calling SerpAPI Flights.");
    //            return new ApiResponse(500, "Unable to reach search provider.HTTP_ERROR");
    //        }
    //    }

    //    private async Task<ApiResponse> HandlePhotoResponse(
    //        HttpResponseMessage httpResponse,
    //        //string cacheKey,
    //        CancellationToken cancellationToken)
    //    {
    //        var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

    //        if (httpResponse.StatusCode == HttpStatusCode.TooManyRequests)
    //        {
    //            _logger.LogWarning("SerpAPI rate limit hit.");
    //            return new ApiResponse((int)httpResponse.StatusCode, "Search rate limit reached. Please wait and retry.RATE_LIMIT");
    //        }

    //        if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
    //        {
    //            _logger.LogError("SerpAPI invalid API key.");
    //            return new ApiResponse((int)httpResponse.StatusCode, "Invalid API key configuration.INVALID_KEY");
    //        }

    //        if (!httpResponse.IsSuccessStatusCode)
    //        {
    //            _logger.LogError("SerpAPI Flights returned {StatusCode}: {Content}", httpResponse.StatusCode, content);
    //            return new ApiResponse((int)httpResponse.StatusCode, $"Search provider error: {httpResponse.StatusCode},PROVIDER_ERROR");
    //        }

    //        var json = JsonNode.Parse(content);
    //        if (json is null)
    //            return new ApiResponse((int)httpResponse.StatusCode, "Empty response from search provider.EMPTY_RESPONSE");

    //        // Check for SerpAPI error field
    //        var errorMsg = json["error"]?.GetValue<string>();
    //        if (!string.IsNullOrEmpty(errorMsg))
    //        {
    //            _logger.LogError("SerpAPI returned error: {Error}", errorMsg);
    //            return new ApiResponse((int)httpResponse.StatusCode, $"{errorMsg} SERP_ERROR");
    //        }
    //        try
    //        {
    //            Console.WriteLine(json);
    //            var response = new FlightSearchResponse
    //            {
    //                SearchId = json["search_metadata"]?["id"]?.GetValue<string>() ?? string.Empty,
    //                currency = json["search_parameters"]?["currency"]?.GetValue<string>() ?? string.Empty,
    //                BestFlights = ParseFlights(json["best_flights"]),
    //                OtherFlights = ParseFlights(json["other_flights"]),
    //                PriceInsights = ParsePriceInsights(json["price_insights"])
    //            };

    //            if (!response.BestFlights.Any() && !response.OtherFlights.Any())
    //            {
    //                _logger.LogInformation("No flight results found.");
    //                return new ApiResultResponse<FlightSearchResponse>(200, response, "No flights found for the given criteria.");
    //            }

    //            //if (_settings.EnableCaching && _cache is not null)
    //            //{
    //            //    var serialized = JsonSerializer.Serialize(response, _jsonOptions);
    //            //    await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
    //            //    {
    //            //        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.CacheDurationMinutes)
    //            //    }, cancellationToken);
    //            //}

    //            return new ApiResultResponse<FlightSearchResponse>(200, response, "Flights retrieved successfully.");
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(ex);
    //            Console.WriteLine(json);

    //            throw ex;
    //        }
    //    }


    //    private Dictionary<string, string> BuildphotoQuery(HotelSearchRequest request)
    //    {
    //        if (request.Children > 0)
    //            if (request.Children != request.ChildrenAges.Count)
    //                throw new Exception("ChildrenAges len should be == Children number");
    //        var p = new Dictionary<string, string>
    //        {
    //            ["engine"] = "google_hotels",
    //            ["api_key"] = _settings.ApiKey,
    //            ["q"] = request.Destination,
    //            ["check_in_date"] = request.CheckInDate,
    //            ["check_out_date"] = request.CheckOutDate,
    //            ["adults"] = request.Adults.ToString(),
    //            ["children"] = request.Children.ToString(),
    //            //["children_ages"] = request.children_ages.ToString(),
    //            ["rooms"] = request.Rooms.ToString(),
    //            ["currency"] = request.Currency.ToUpperInvariant(),
    //            //["sort_by"] = ((int)request.SortBy).ToString(),
    //            ["gl"] = request.Gl.ToLowerInvariant(),
    //            ["hl"] = request.Hl.ToLowerInvariant()
    //        };

    //        //if (request.MinRating.HasValue)
    //        //p["rating"] = request.MinRating.Value.ToString();

    //        if (request.MinPrice.HasValue)
    //            p["min_price"] = request.MinPrice.Value.ToString("F0");

    //        if (request.MaxPrice.HasValue)
    //            p["max_price"] = request.MaxPrice.Value.ToString("F0");

    //        if (request.Amenities.Any())
    //            p["amenities"] = string.Join(",", request.Amenities);
    //        if (request.ChildrenAges.Any())
    //            p["children_ages"] = string.Join(",", request.ChildrenAges);

    //        return p;
    //    }

    //    private string BuildUrl(Dictionary<string, string> parameters)
    //    {
    //        var query = string.Join("&", parameters.Select(kv =>
    //            $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
    //        return $"{_settings.BaseUrl}?{query}";
    //    }

    //}

}
