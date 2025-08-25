using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Server.Data
{
    public class ForecastService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.openweathermap.org/data/2.5/forecast";

        public ForecastService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenWeatherMap:ApiKey"] ?? throw new ArgumentNullException("OpenWeatherMap:ApiKey not configured");
        }

        public async Task<OpenWeatherForecastResponse?> GetForecastByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City name is required", nameof(city));

            var url = $"{_baseUrl}?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric";
            return await GetForecastAsync(url);
        }

        public async Task<OpenWeatherForecastResponse?> GetForecastByCoordinatesAsync(double lat, double lon)
        {
            if (lat < -90 || lat > 90)
                throw new ArgumentException("Latitude must be between -90 and 90", nameof(lat));
            
            if (lon < -180 || lon > 180)
                throw new ArgumentException("Longitude must be between -180 and 180", nameof(lon));

            var url = $"{_baseUrl}?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";
            return await GetForecastAsync(url);
        }

        private async Task<OpenWeatherForecastResponse?> GetForecastAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<OpenWeatherErrorResponse>(content);
                        throw new OpenWeatherMapException(
                            errorResponse?.Message ?? "OpenWeatherMap API error",
                            response.StatusCode,
                            errorResponse?.Cod ?? "unknown"
                        );
                    }
                    catch (JsonException)
                    {
                        throw new OpenWeatherMapException(
                            $"API request failed with status {(int)response.StatusCode}",
                            response.StatusCode,
                            ((int)response.StatusCode).ToString()
                        );
                    }
                }

                var forecast = JsonSerializer.Deserialize<OpenWeatherForecastResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return forecast;
            }
            catch (HttpRequestException ex)
            {
                throw new OpenWeatherMapException("Network error occurred", HttpStatusCode.ServiceUnavailable, "network_error", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new OpenWeatherMapException("Request timeout", HttpStatusCode.RequestTimeout, "timeout", ex);
            }
            catch (JsonException ex)
            {
                throw new OpenWeatherMapException("Invalid response format", HttpStatusCode.BadRequest, "invalid_format", ex);
            }
        }

        public string BuildCityUrl(string city)
        {
            return $"{_baseUrl}?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric";
        }

        public string BuildCoordinatesUrl(double lat, double lon)
        {
            return $"{_baseUrl}?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";
        }
    }

    public class OpenWeatherMapException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ErrorCode { get; }

        public OpenWeatherMapException(string message, HttpStatusCode statusCode, string errorCode, Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }

    // Using the OpenWeatherErrorResponse from ForecastModels.cs
}
