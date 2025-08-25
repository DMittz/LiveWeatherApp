using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Client.Models;

namespace Client.Services
{
    public class WeatherService
    {
        private readonly HttpClient _http;
        public WeatherService(HttpClient http) => _http = http;

        // City-based calls used by your pages
        public async Task<OpenWeatherResponse?> GetCurrentAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) return null;
            var encoded = Uri.EscapeDataString(city);
            var candidates = new[]
            {
                $"/api/weather/current?city={encoded}",
                $"/api/weather/current/{encoded}",
                $"/api/weather?city={encoded}",
                $"/api/weather/{encoded}"
            };

            foreach (var path in candidates)
            {
                try
                {
                    var resp = await _http.GetAsync(path);
                    if (!resp.IsSuccessStatusCode) continue;
                    var body = await resp.Content.ReadFromJsonAsync<OpenWeatherResponse>();
                    if (body != null) return body;
                }
                catch
                {
                    // ignore and try next candidate
                }
            }
            return null;
        }

        public async Task<ForecastResponse?> GetForecastAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) return null;
            var encoded = Uri.EscapeDataString(city);
            var candidates = new[]
            {
                $"/api/weather/forecast?city={encoded}",
                $"/api/weather/forecast/{encoded}",
                $"/api/weather/forecastcity?city={encoded}",
                $"/api/weather/forecast/{encoded}"
            };

            foreach (var path in candidates)
            {
                try
                {
                    var resp = await _http.GetAsync(path);
                    if (!resp.IsSuccessStatusCode) continue;
                    var body = await resp.Content.ReadFromJsonAsync<ForecastResponse>();
                    if (body != null) return body;
                }
                catch
                {
                    // ignore and try next
                }
            }
            return null;
        }

        // coordinate-based methods (kept from previous)
        public async Task<OpenWeatherResponse?> GetCurrentByCoordinatesAsync(double lat, double lon)
        {
            var candidates = new[]
            {
                $"/api/weather/current/coords?lat={lat}&lon={lon}",
                $"/api/weather/current?lat={lat}&lon={lon}",
                $"/api/weather?lat={lat}&lon={lon}",
                $"/api/weather/current/coords/{lat}/{lon}",
                $"/api/weather/current/{lat}/{lon}"
            };

            foreach (var path in candidates)
            {
                try
                {
                    var resp = await _http.GetAsync(path);
                    if (!resp.IsSuccessStatusCode) continue;
                    var body = await resp.Content.ReadFromJsonAsync<OpenWeatherResponse>();
                    if (body != null) return body;
                }
                catch { }
            }

            return null;
        }

        public async Task<ForecastResponse?> GetForecastByCoordinatesAsync(double lat, double lon)
        {
            var candidates = new[]
            {
                $"/api/weather/forecast/coords?lat={lat}&lon={lon}",
                $"/api/weather/forecast?lat={lat}&lon={lon}",
                $"/api/weather/forecast/{lat}/{lon}",
                $"/api/weather/forecastcoords?lat={lat}&lon={lon}",
                $"/api/weather/forecastcoords/{lat}/{lon}"
            };

            foreach (var path in candidates)
            {
                try
                {
                    var resp = await _http.GetAsync(path);
                    if (!resp.IsSuccessStatusCode) continue;
                    var body = await resp.Content.ReadFromJsonAsync<ForecastResponse>();
                    if (body != null) return body;
                }
                catch { }
            }

            return null;
        }
    }
}
