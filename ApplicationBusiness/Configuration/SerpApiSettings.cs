using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Configuration
{
    public class SerpApiSettings
    {
        public const string SectionName = "SerpApi";

        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://serpapi.com/search.json";
        public int TimeoutSeconds { get; set; } = 30;
        public int RetryCount { get; set; } = 3;
        public int RetryDelayMilliseconds { get; set; } = 500;
        public bool EnableCaching { get; set; } = true;
        public int CacheDurationMinutes { get; set; } = 15;
    }
}
