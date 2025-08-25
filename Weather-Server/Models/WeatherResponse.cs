using System;
using System.Collections.Generic;

namespace Server.Models
{
    public class WeatherResponse
    {
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public int Humidity { get; set; }
        public int Pressure { get; set; }
        public double WindSpeed { get; set; }
        public int WindDeg { get; set; }
        public int Clouds { get; set; }
        public int Visibility { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class DailyForecast
    {
        public DateTime Date { get; set; }
        public double AverageTemp { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class ForecastResponse
    {
        public string City { get; set; } = string.Empty;
        public List<DailyForecast> DailyForecasts { get; set; } = new();
    }
}
