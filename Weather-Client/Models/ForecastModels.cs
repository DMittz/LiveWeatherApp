using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Client.Models
{
    public class ForecastResponse
    {
        [JsonPropertyName("city")]
        public CityInfo? City { get; set; }

        [JsonPropertyName("list")]
        public List<ForecastItem>? List { get; set; }
    }

    public class ForecastItem
    {
        [JsonPropertyName("dt")]
        public long Dt { get; set; }

        // Page code expects Dt_Txt
        [JsonPropertyName("dt_txt")]
        public string? Dt_Txt { get; set; }

        [JsonPropertyName("main")]
        public MainInfo? Main { get; set; }

        // pages expect array-style access Weather[0]
        [JsonPropertyName("weather")]
        public WeatherInfo[]? Weather { get; set; }

        [JsonPropertyName("wind")]
        public WindInfo? Wind { get; set; }

        [JsonPropertyName("clouds")]
        public CloudsInfo? Clouds { get; set; }

        [JsonPropertyName("rain")]
        public RainInfo? Rain { get; set; }

        [JsonPropertyName("pop")]
        public double Pop { get; set; }    }
    
    public class MainInfo
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        // pages use Temp_Min / Temp_Max identifiers
        [JsonPropertyName("temp_min")]
        public double Temp_Min { get; set; }

        [JsonPropertyName("temp_max")]
        public double Temp_Max { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }

        [JsonPropertyName("pressure")]
        public int Pressure { get; set; }
    }

    public class WeatherInfo
    {
        [JsonPropertyName("main")]
        public string? Main { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
    }

    public class WindInfo
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        [JsonPropertyName("deg")]
        public int Deg { get; set; }
    }

    public class CloudsInfo
    {
        [JsonPropertyName("all")]
        public int All { get; set; }
    }

    public class RainInfo
    {
        [JsonPropertyName("3h")]
        public double? _3h { get; set; }
    }

    public class CityInfo
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("coord")]
        public CoordInfo? Coord { get; set; }
    }

    public class CoordInfo
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }

    // UI-friendly daily aggregation used by your pages
    public class DailyForecast
    {
        public DateTime Date { get; set; }
        public double AvgTemp { get; set; }
        public double MinTemp { get; set; }
        public double MaxTemp { get; set; }
        public string MainCondition { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public double Pop { get; set; }
        public double Rain3h { get; set; }
    }
}
