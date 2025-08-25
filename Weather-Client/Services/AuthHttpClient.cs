using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Client.Services;

namespace Client.Services
{
    public class AuthHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly SupabaseAuthService _authService;

        public AuthHttpClient(HttpClient httpClient, SupabaseAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
            
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            // Add JWT token if user is logged in
            if (_authService.IsLoggedIn)
            {
                var token = _authService.GetJwt();
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return await _httpClient.SendAsync(request, cancellationToken);
        }

    public async Task<T?> GetFromJsonAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await SendAsync(request, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                throw new HttpRequestException($"API request failed: {response.StatusCode}");
            }
            
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            
            return await response.Content.ReadFromJsonAsync<T>(jsonOptions, cancellationToken);
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetFromJsonAsync: {ex.Message}");
            throw;
        }
    }
    }
}
