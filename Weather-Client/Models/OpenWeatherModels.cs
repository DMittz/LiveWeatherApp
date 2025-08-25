using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Client.Models
{
    public class OpenWeatherResponse
    {
        public string? Name { get; set; }

        // reuse MainInfo from ForecastModels
        public MainInfo? Main { get; set; }

        // pages access Weather[0]
        public WeatherInfo[]? Weather { get; set; }

        public WindInfo? Wind { get; set; }

        public double? Pop { get; set; }

        public RainInfo? Rain { get; set; }

        // pages reference Sys.Sunrise / Sys.Sunset
        public SysInfo? Sys { get; set; }
    }

    public class SysInfo
    {
        [JsonPropertyName("sunrise")]
        public long Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public long Sunset { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }
}