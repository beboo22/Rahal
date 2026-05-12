using ApplicationBusiness.Dtos.Activity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ApplicationBusiness.Configuration;
using Domain.BaseResponce;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using FuzzySharp;
using System.Text.RegularExpressions;

namespace ApplicationBusiness.Abstraction.SerpApiService.Activity
{
    public sealed class ActivityService : IActivityService
    {
        private const int OptionsPerNeed = 3; // بنرجع 3 خيارات عشان اليوزر يختار
        private const int DefaultZoom = 14;
        private const int SearchRadius = 2000;

        private static readonly Dictionary<ActivityNeed, string> NeedQueryMap = new()
        {
            [ActivityNeed.Breakfast] = "breakfast restaurant",
            [ActivityNeed.Lunch] = "lunch restaurant",
            [ActivityNeed.Dinner] = "dinner restaurant",
            [ActivityNeed.Cafe] = "cafe coffee shop",
            [ActivityNeed.Activities] = "tourist attraction",
            [ActivityNeed.Mall] = "shopping mall",
            [ActivityNeed.Pharmacy] = "pharmacy",
            [ActivityNeed.Supermarket] = "supermarket",
            [ActivityNeed.Hospital] = "hospital"
        };

        private readonly HttpClient _httpClient;
        private readonly SerpApiSettings _settings;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _cache;
        private readonly ILogger<ActivityService> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public ActivityService(
            HttpClient httpClient,
            IOptions<SerpApiSettings> settings,
            IConnectionMultiplexer redis,
            ILogger<ActivityService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _redis = redis;
            _cache = redis.GetDatabase();
            _logger = logger;
        }

        public async Task<ApiResponse> CreateActivityByDayAsync(CreateActivityPlanRequest request, int userId, CancellationToken cancellationToken = default)
        {
            if (request.Days == null || !request.Days.Any())
                return new ApiResponse(400, "No days provided.");

            var planResponse = new ActivityPlanResponse();

            foreach (var day in request.Days.OrderBy(d => d.DayNumber))
            {
                var dayResult = await ProcessDayAsync(day, userId, cancellationToken);
                planResponse.Days.Add(dayResult);
            }

            return new ApiResultResponse<ActivityPlanResponse>(200, planResponse, "Itinerary generated successfully.");
        }

        private async Task<DayActivityResponse> ProcessDayAsync(DayActivityRequest day, int userId, CancellationToken cancellationToken)
        {
            var dayResponse = new DayActivityResponse { DayNumber = day.DayNumber, City = day.City };
            GpsCoordinates? baseLocation = null;

            foreach (var need in day.Needs)
            {
                var needKey = need.ToString();
                var cacheKey = ActivityCacheKeys.Exact(day.City, need, baseLocation);
                var trackerKey = $"user-tracker:{userId}:{cacheKey}";

                // 1. محاولة جلب البيانات من الكاش (لو يوزر جديد)
                var cachedJson = await _cache.StringGetAsync(cacheKey);
                var hasUserSeenBefore = await _cache.KeyExistsAsync(trackerKey);

                if (!cachedJson.IsNull && !hasUserSeenBefore && _settings.EnableCaching)
                {
                    _logger.LogInformation("Cache Hit for User {Id}", userId);
                    var cachedOptions = JsonSerializer.Deserialize<List<PlaceOption>>(cachedJson!, _jsonOptions);
                    dayResponse.Results[needKey] = cachedOptions!;
                    await _cache.StringSetAsync(trackerKey, "seen", TimeSpan.FromDays(1));

                    if (baseLocation == null) baseLocation = cachedOptions?.FirstOrDefault()?.GpsCoordinates;
                    continue;
                }

                // 2. محاولة Fuzzy Search لو مفيش Exact Match
                if (cachedJson.IsNull && _settings.EnableCaching)
                {
                    var fuzzyOptions = await FindSimilarActivityAsync(cacheKey);
                    if (fuzzyOptions != null)
                    {
                        dayResponse.Results[needKey] = fuzzyOptions;
                        if (baseLocation == null) baseLocation = fuzzyOptions.FirstOrDefault()?.GpsCoordinates;
                        continue;
                    }
                }

                // 3. نكلم SerpApi مع ضمان التنوع (Refresh Logic)
                int offset = 0;
                List<PlaceOption> freshOptions;
                bool isDuplicate;

                do
                {
                    var queryParams = BuildMapsQuery(need, day.City, baseLocation, day.Language, day.CountryCode);
                    if (offset > 0) queryParams["start"] = offset.ToString();

                    var url = BuildUrl(queryParams);
                    var response = await _httpClient.GetAsync(url, cancellationToken);
                    freshOptions = await ParseMapsResponseAsync(response, cancellationToken);

                    // نختبر التكرار: هل النتيجة الجديدة موجودة في الكاش القديم؟
                    isDuplicate = !cachedJson.IsNull && freshOptions.Any() &&
                                  CheckIfDuplicate(cachedJson!, freshOptions[0].PlaceId);

                    if (isDuplicate)
                    {
                        _logger.LogInformation("Duplicate detected. Skipping page (start={Offset})", offset);
                        offset += 20;
                    }
                } while (isDuplicate && offset < 60);

                // 4. تحديث الكاش والعرض
                if (freshOptions.Any())
                {
                    // 1. جلب الداتا القديمة اللي موجودة فعلاً في الكاش
                    var existingCachedData = await _cache.StringGetAsync(cacheKey);
                    List<PlaceOption> finalOptionsList;

                    if (!existingCachedData.IsNull)
                    {
                        // 2. لو في داتا قديمة، فكها وحطها في List
                        finalOptionsList = JsonSerializer.Deserialize<List<PlaceOption>>(existingCachedData!, _jsonOptions) ?? new List<PlaceOption>();

                        // 3. ضيف عليها الـ Options الجديدة (مع التأكد إن مفيش تكرار بالـ PlaceId)
                        foreach (var newOpt in freshOptions)
                        {
                            if (!finalOptionsList.Any(x => x.PlaceId == newOpt.PlaceId))
                            {
                                finalOptionsList.Add(newOpt);
                            }
                        }
                    }
                    else
                    {
                        // لو الكاش فاضي، استعمل الداتا الجديدة بس
                        finalOptionsList = freshOptions;
                    }

                    var serialized = JsonSerializer.Serialize(finalOptionsList, _jsonOptions);
                    await _cache.StringSetAsync(cacheKey, serialized, TimeSpan.FromMinutes(_settings.CacheDurationMinutes));

                    // 5. سجل إن المستخدم شاف النسخة دي (Tracker)
                    await _cache.StringSetAsync(trackerKey, "seen", TimeSpan.FromDays(1));

                    //var serialized = JsonSerializer.Serialize(freshOptions, _jsonOptions);
                    ////await _cache.StringAppendAsync(cacheKey, serialized, TimeSpan.FromHours(_settings.CacheDurationMinutes));
                    //await _cache.StringAppendAsync(cacheKey, serialized);
                    ////await _cache.StringAppendAsync(trackerKey, "seen", TimeSpan.FromDays(1));
                    //await _cache.StringAppendAsync(trackerKey, "seen");
                    if (baseLocation == null) baseLocation = freshOptions[0].GpsCoordinates;
                }

                dayResponse.Results[needKey] = freshOptions;
            }
            dayResponse.BaseLocation = baseLocation;
            return dayResponse;
        }

        // --- المساعدين (Helpers) ---

        private bool CheckIfDuplicate(string cachedJson, string newPlaceId)
        {
            var cached = JsonSerializer.Deserialize<List<PlaceOption>>(cachedJson, _jsonOptions);
            return cached?.Any(x => x.PlaceId == newPlaceId) ?? false;
        }

        private async Task<List<PlaceOption>?> FindSimilarActivityAsync(string currentSearchKey)
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var allKeys = server.Keys(pattern: "activities:*").Select(k => k.ToString()).ToList();
            if (!allKeys.Any()) return null;

            var normalizedCurrent = NormalizeKey(currentSearchKey);
            var best = allKeys.Select(k => new { Key = k, Score = Fuzz.Ratio(normalizedCurrent, NormalizeKey(k)) })
                              .OrderByDescending(x => x.Score).FirstOrDefault();

            if (best != null && best.Score >= 85)
            {
                var data = await _cache.StringGetAsync(best.Key);
                return JsonSerializer.Deserialize<List<PlaceOption>>(data!, _jsonOptions);
            }
            return null;
        }

        private static string NormalizeKey(string key) => Regex.Replace(key.ToLower(), @"[:\s\d\.]+", "");

        private Dictionary<string, string> BuildMapsQuery(ActivityNeed need, string city, GpsCoordinates? loc, string lang, string country)
        {
            var query = new Dictionary<string, string>
            {
                ["engine"] = "google_maps",
                ["api_key"] = _settings.ApiKey,
                ["type"] = "search",
                ["hl"] = lang,
                ["gl"] = country
            };

            if (loc == null) query["q"] = $"{NeedQueryMap[need]} in {city}";
            else
            {
                query["q"] = NeedQueryMap[need];
                query["ll"] = $"@{loc.Latitude:F6},{loc.Longitude:F6},{DefaultZoom}z";
                query["radius"] = SearchRadius.ToString();
            }
            return query;
        }

        private string BuildUrl(Dictionary<string, string> p) => $"{_settings.BaseUrl}?{string.Join("&", p.Select(kv => $"{kv.Key}={kv.Value}"))}";

        private async Task<List<PlaceOption>> ParseMapsResponseAsync(HttpResponseMessage res, CancellationToken ct)
        {
            if (!res.IsSuccessStatusCode) return new();
            var content = await res.Content.ReadAsStringAsync(ct);
            var json = JsonNode.Parse(content);
            var results = json?["local_results"] as JsonArray;

            var list = new List<PlaceOption>();
            if (results == null) return list;

            foreach (var item in results.Take(OptionsPerNeed))
            {
                list.Add(new PlaceOption
                {
                    Position = item["position"]?.GetValue<int>() ?? 0,

                    Title = item["title"]?.GetValue<string>() ?? string.Empty,

                    PlaceId = item["place_id"]?.GetValue<string>() ?? string.Empty,

                    DataId = item["data_id"]?.GetValue<string>() ?? string.Empty,

                    Rating = item["rating"]?.GetValue<double>() ?? 0,

                    Reviews = item["reviews"]?.GetValue<int>() ?? 0,

                    Type = item["type"]?.GetValue<string>() ?? string.Empty,

                    Address = item["address"]?.GetValue<string>() ?? string.Empty,

                    Phone = item["phone"]?.GetValue<string>() ?? string.Empty,

                    Website = item["website"]?.GetValue<string>() ?? string.Empty,

                    Thumbnail = item["thumbnail"]?.GetValue<string>() ?? string.Empty,

                    OpenState = item["open_state"]?.GetValue<string>() ?? string.Empty,

                    Hours = item["hours"]?.GetValue<string>() ?? string.Empty,

                    Price = item["price"]?.GetValue<string>() ?? string.Empty,

                    Description = item["description"]?.GetValue<string>() ?? string.Empty,
                    GpsCoordinates = new GpsCoordinates
                    {
                        Latitude = (double)(item?["gps_coordinates"]?["latitude"] ?? 0),
                        Longitude = (double)(item?["gps_coordinates"]?["longitude"] ?? 0)
                    },
                    ServiceOptions = ParseServiceOptions(item["service_options"])
                });
            }
            return list;
        }

        private static List<string> ParseServiceOptions(JsonNode? node)
        {
            var result = new List<string>();
            if (node is not JsonObject obj) return result;

            foreach (var kvp in obj)
            {
                var flag = kvp.Value?.GetValue<bool>() ?? false;
                if (flag)
                    result.Add(kvp.Key.Replace("_", " "));
            }
            return result;
        }
    }

    internal static class ActivityCacheKeys
    {
        public static string Exact(string city, ActivityNeed need, GpsCoordinates? loc)
        {
            var prefix = $"activities:{city.ToLower()}:{need.ToString().ToLower()}";
            if (loc == null) return prefix;
            return $"{prefix}:{Math.Round(loc.Latitude, 3)}:{Math.Round(loc.Longitude, 3)}";
        }
    }
}