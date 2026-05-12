// ============================================================
//  ActivityService — Data Transfer Objects
//  File: DTOs/ActivityDtos.cs
// ============================================================
using System.Text.Json.Serialization;
namespace ApplicationBusiness.Dtos.Activity
{
    // ══════════════════════════════════════════════════════════
    //  ENUMS
    // ══════════════════════════════════════════════════════════

    /// <summary>
    /// All possible "need" categories a user can request for a single day.
    /// Maps directly to a Google Maps search query keyword.
    /// </summary>
    public enum ActivityNeed
    {
        Breakfast,
        Lunch,
        Dinner,
        Cafe,
        Activities,     // Tourist attractions, museums, parks …
        Mall,
        Pharmacy,
        Supermarket,
        Hospital
    }

    // ══════════════════════════════════════════════════════════
    //  REQUEST
    // ══════════════════════════════════════════════════════════

    /// <summary>
    /// Top-level request: a full multi-day trip plan.
    /// </summary>
    public sealed class CreateActivityPlanRequest
    {
        /// <summary>Ordered list of day configs (index 0 = Day 1).</summary>
        public List<DayActivityRequest> Days { get; set; } = [];
    }

    /// <summary>
    /// Configuration for a single trip day.
    /// </summary>
    public sealed class DayActivityRequest
    {
        /// <summary>1-based day number. Used for display only.</summary>
        public int DayNumber { get; set; }

        /// <summary>
        /// City / area for this day (e.g. "Cairo", "Alexandria").
        /// Used as the textual anchor for geo-coding and as the first
        /// ll-bias when no base location has been established yet.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Ordered list of needs for this day.
        /// Order matters: the FIRST resolved place becomes the
        /// Base Location that biases every subsequent search.
        /// </summary>
        public List<ActivityNeed> Needs { get; set; } = [];

        /// <summary>
        /// Optional: search language (e.g. "en", "ar"). Defaults to "en".
        /// </summary>
        public string Language { get; set; } = "en";

        /// <summary>
        /// Optional: search country code (e.g. "eg"). Defaults to "eg".
        /// </summary>
        public string CountryCode { get; set; } = "eg";
    }

    // ══════════════════════════════════════════════════════════
    //  RESPONSE
    // ══════════════════════════════════════════════════════════

    /// <summary>
    /// Full trip plan response — one entry per day.
    /// </summary>
    public sealed class ActivityPlanResponse
    {
        public List<DayActivityResponse> Days { get; set; } = [];
    }

    /// <summary>
    /// Results for one trip day.
    /// </summary>
    public sealed class DayActivityResponse
    {
        public int DayNumber { get; set; }
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Results keyed by ActivityNeed. Each key holds ≤3 options
        /// so the user can pick their favourite.
        /// </summary>
        public Dictionary<string, List<PlaceOption>> Results { get; set; } = [];

        /// <summary>
        /// The GPS coordinate that was used to bias all subsequent
        /// searches after the first need was resolved.
        /// </summary>
        public GpsCoordinates? BaseLocation { get; set; }
    }

    /// <summary>
    /// One suggested place (returned in a set of ≤3 per need).
    /// Field names mirror the SerpApi google_maps local_results schema.
    /// </summary>
    public sealed class PlaceOption
    {
        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; } = string.Empty;

        [JsonPropertyName("data_id")]
        public string DataId { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("reviews")]
        public int Reviews { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;

        [JsonPropertyName("website")]
        public string Website { get; set; } = string.Empty;

        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; } = string.Empty;

        [JsonPropertyName("gps_coordinates")]
        public GpsCoordinates? GpsCoordinates { get; set; }

        [JsonPropertyName("open_state")]
        public string OpenState { get; set; } = string.Empty;

        [JsonPropertyName("hours")]
        public string Hours { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public string Price { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("service_options")]
        public List<string> ServiceOptions { get; set; } = [];
    }

    public sealed class GpsCoordinates
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}
