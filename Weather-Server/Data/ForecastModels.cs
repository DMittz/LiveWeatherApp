using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Server.Data
{
    // Root response object for 5-day/3-hour forecast
    public class OpenWeatherForecastResponse
    {
        [JsonPropertyName("cod")]
        public string? Cod { get; set; }

        [JsonPropertyName("message")]
        public int? Message { get; set; }

        [JsonPropertyName("cnt")]
        public int? Cnt { get; set; }

        [JsonPropertyName("list")]
        public List<ForecastListItem>? List { get; set; }

        [JsonPropertyName("city")]
        public CityInfo? City { get; set; }
    }

    public class CityInfo
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("coord")]
        public CoordInfo? Coord { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("population")]
        public int? Population { get; set; }

        [JsonPropertyName("timezone")]
        public int? Timezone { get; set; }

        [JsonPropertyName("sunrise")]
        public long? Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public long? Sunset { get; set; }
    }

    public class CoordInfo
    {
        [JsonPropertyName("lat")]
        public double? Lat { get; set; }

        [JsonPropertyName("lon")]
        public double? Lon { get; set; }
    }

    public class ForecastListItem
    {
        [JsonPropertyName("dt")]
        public long Dt { get; set; }

        [JsonPropertyName("dt_txt")]
        public string? DtTxt { get; set; }

        [JsonPropertyName("main")]
        public ForecastMainInfo? Main { get; set; }

        [JsonPropertyName("weather")]
        public List<ForecastWeatherInfo>? Weather { get; set; }

        [JsonPropertyName("clouds")]
        public ForecastCloudsInfo? Clouds { get; set; }

        [JsonPropertyName("wind")]
        public ForecastWindInfo? Wind { get; set; }

        [JsonPropertyName("visibility")]
        public int? Visibility { get; set; }

        [JsonPropertyName("pop")]
        public double? Pop { get; set; }

        [JsonPropertyName("rain")]
        public ForecastRainInfo? Rain { get; set; }

        [JsonPropertyName("sys")]
        public ForecastSysInfo? Sys { get; set; }
    }

    public class ForecastMainInfo
    {
        [JsonPropertyName("temp")]
        public double? Temp { get; set; }

        [JsonPropertyName("feels_like")]
        public double? FeelsLike { get; set; }

        [JsonPropertyName("temp_min")]
        public double? TempMin { get; set; }

        [JsonPropertyName("temp_max")]
        public double? TempMax { get; set; }

        [JsonPropertyName("pressure")]
        public int? Pressure { get; set; }

        [JsonPropertyName("sea_level")]
        public int? SeaLevel { get; set; }

        [JsonPropertyName("grnd_level")]
        public int? GrndLevel { get; set; }

        [JsonPropertyName("humidity")]
        public int? Humidity { get; set; }

        [JsonPropertyName("temp_kf")]
        public double? TempKf { get; set; }
    }

    public class ForecastWeatherInfo
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("main")]
        public string? Main { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
    }

    public class ForecastCloudsInfo
    {
        [JsonPropertyName("all")]
        public int? All { get; set; }
    }

    public class ForecastWindInfo
    {
        [JsonPropertyName("speed")]
        public double? Speed { get; set; }

        [JsonPropertyName("deg")]
        public int? Deg { get; set; }

        [JsonPropertyName("gust")]
        public double? Gust { get; set; }
    }

    public class ForecastRainInfo
    {
        [JsonPropertyName("3h")]
        public double? ThreeH { get; set; }
    }

    public class ForecastSysInfo
    {
        [JsonPropertyName("pod")]
        public string? Pod { get; set; }
    }

    public class OpenWeatherErrorResponse
    {
        [JsonPropertyName("cod")]
        public string? Cod { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    // Legacy models (to be removed after migration)
    public class ForecastResponse
    {
        public string City { get; set; } = string.Empty;
        public List<DailyForecast> DailyForecasts { get; set; } = new();
    }

    public class DailyForecast
    {
        public DateTime Date { get; set; }
        public double AverageTemp { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
