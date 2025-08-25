using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Client.Models;

namespace Client.Services
{
    public class ForecastService
    {
        private readonly HttpClient _httpClient;

        public ForecastService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ForecastResponse?> GetForecastByCityAsync(string city)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ForecastResponse>($"/api/forecast/city/{Uri.EscapeDataString(city)}");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error fetching forecast for city '{city}': {ex.Message}");
            }
        }

        public async Task<ForecastResponse?> GetForecastByCoordinatesAsync(double lat, double lon)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ForecastResponse>($"/api/forecast/coords?lat={lat}&lon={lon}");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error fetching forecast for coordinates ({lat}, {lon}): {ex.Message}");
            }
        }
    }
}
