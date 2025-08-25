using System;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // added for Skip/Take/Average


namespace Server.Data
{
    public class OpenWeatherMapService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenWeatherMapService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["OpenWeatherMap:ApiKey"] ?? "";
        }

        public async Task<OpenWeatherResponse?> GetWeatherByCityAsync(string city)
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
            try
            {
                var response = await _httpClient.GetAsync(url);
                var rawJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"OpenWeatherMap raw response: {rawJson}");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"OpenWeatherMap API error: {response.StatusCode} {rawJson}");
                    return null;
                }
                try
                {
                    var result = System.Text.Json.JsonSerializer.Deserialize<OpenWeatherResponse>(rawJson);
                    if (result == null)
                    {
                        Console.WriteLine($"Deserialization returned null. Raw JSON: {rawJson}");
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Deserialization error: {ex.Message}\nRaw JSON: {rawJson}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HTTP error: {ex.Message}");
                return null;
            }
        }

        // NEW: fetch current weather by coordinates (returns only current weather, not 3-hour forecast)
        public async Task<OpenWeatherResponse?> GetWeatherByCoordinatesAsync(double lat, double lon)
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";
            try
            {
                var response = await _httpClient.GetAsync(url);
                var rawJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"OpenWeatherMap raw response (coords): {rawJson}");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"OpenWeatherMap API error: {response.StatusCode} {rawJson}");
                    return null;
                }

                try
                {
                    var result = System.Text.Json.JsonSerializer.Deserialize<OpenWeatherResponse>(rawJson);
                    if (result == null)
                    {
                        Console.WriteLine($"Deserialization returned null. Raw JSON: {rawJson}");
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Deserialization error: {ex.Message}\nRaw JSON: {rawJson}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HTTP error: {ex.Message}");
                return null;
            }
        }

        public async Task<ForecastResponse?> GetForecastByCityAsync(string city)
        {
            var url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric";
            try
            {
                var response = await _httpClient.GetAsync(url);
                var rawJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"OpenWeatherMap forecast response: {rawJson}");
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"OpenWeatherMap API error: {response.StatusCode} {rawJson}");
                    return null;
                }

                try
                {
                    var apiResponse = System.Text.Json.JsonSerializer.Deserialize<OpenWeatherForecastResponse>(rawJson);
                    if (apiResponse?.List == null || apiResponse.List.Count == 0)
                    {
                        Console.WriteLine($"Forecast deserialization returned null or empty. Raw JSON: {rawJson}");
                        return null;
                    }

                    return ProcessForecastData(apiResponse, city);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Forecast deserialization error: {ex.Message}\nRaw JSON: {rawJson}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HTTP error: {ex.Message}");
                return null;
            }
        }

        private ForecastResponse ProcessForecastData(OpenWeatherForecastResponse apiResponse, string city)
        {
            var dailyForecasts = new List<DailyForecast>();
            
            // Group forecast data by day (every 8 entries = 24 hours)
            for (int day = 0; day < 5; day++)
            {
                int startIndex = day * 8;
                if (startIndex + 8 > apiResponse.List.Count) break;

                var dayEntries = apiResponse.List.Skip(startIndex).Take(8).ToList();
                
                if (dayEntries.Count == 0) continue;

                // Calculate average temperature
                var avgTemp = dayEntries.Average(e => e.Main?.Temp ?? 0);
                
                // Get most common condition (first entry's condition for simplicity)
                var condition = dayEntries.FirstOrDefault()?.Weather?.FirstOrDefault()?.Main ?? "Unknown";
                var icon = dayEntries.FirstOrDefault()?.Weather?.FirstOrDefault()?.Icon ?? "01d";
                
                // Calculate date (API returns data in 3-hour intervals)
                var date = DateTimeOffset.FromUnixTimeSeconds(dayEntries[0].Dt).DateTime;

                dailyForecasts.Add(new DailyForecast
                {
                    Date = date,
                    AverageTemp = Math.Round(avgTemp, 1),
                    Condition = condition,
                    Icon = icon
                });
            }

            return new ForecastResponse
            {
                City = city,
                DailyForecasts = dailyForecasts.Take(5).ToList()
            };
        }
    }

    public class OpenWeatherResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("main")]
        public MainInfo? Main { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("weather")]
        public List<WeatherInfo>? Weather { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }
    }
    public class MainInfo
    {
        [System.Text.Json.Serialization.JsonPropertyName("temp")]
        public double Temp { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("temp_min")]
        public double Temp_Min { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("temp_max")]
        public double Temp_Max { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }
    public class WeatherInfo
    {
        [System.Text.Json.Serialization.JsonPropertyName("main")]
        public string? Main { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("description")]
        public string? Description { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("icon")]
        public string? Icon { get; set; }
    }
}
