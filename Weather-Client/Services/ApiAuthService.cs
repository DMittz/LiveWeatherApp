using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.JSInterop;
using Microsoft.Extensions.Configuration;

namespace Client.Services
{
    public class ApiAuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private readonly IConfiguration _config;
        private string? _jwt;

        public User? CurrentUser { get; private set; } // added so components that reference CurrentUser compile
        public bool IsLoggedIn => !string.IsNullOrEmpty(_jwt);
        public string? UserEmail => CurrentUser?.Email;
        public event Action? OnAuthStateChanged;

        public ApiAuthService(HttpClient http, IJSRuntime js, IConfiguration config)
        {
            _http = http;
            _js = js;
            _config = config;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var json = await _js.InvokeAsync<string>("localStorage.getItem", "api.jwt");
                if (!string.IsNullOrEmpty(json))
                {
                    _jwt = json;
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);

                    // optional: attempt to fetch user profile from backend if endpoint exists
                    try
                    {
                        var me = await _http.GetFromJsonAsync<UserProfile>("/api/auth/me");
                        if (me != null) CurrentUser = new User { Email = me.Email, Id = me.Id };
                    }
                    catch { /* ignore if backend doesn't provide /me */ }

                    OnAuthStateChanged?.Invoke();
                }
            }
            catch { /* swallow */ }
        }

        public string? GetJwt() => _jwt;

        public async Task<bool> SignInAsync(string email, string password)
        {
            var payload = new { Email = email, Password = password };
            var resp = await _http.PostAsJsonAsync("/api/auth/login", payload);
            if (!resp.IsSuccessStatusCode) return false;

            // try to read token using common property names
            try
            {
                var doc = await resp.Content.ReadFromJsonAsync<JsonElement>();
                string? token = null;
                if (doc.ValueKind == JsonValueKind.Object)
                {
                    if (doc.TryGetProperty("token", out var t)) token = t.GetString();
                    else if (doc.TryGetProperty("access_token", out var t2)) token = t2.GetString();
                    else if (doc.TryGetProperty("accessToken", out var t3)) token = t3.GetString();
                    else if (doc.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object && data.TryGetProperty("token", out var dt)) token = dt.GetString();
                }

                if (string.IsNullOrEmpty(token))
                {
                    // fallback: try to read plain string body
                    var raw = await resp.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(raw)) token = raw.Trim('"');
                }

                if (string.IsNullOrEmpty(token)) return false;

                _jwt = token;
                await _js.InvokeVoidAsync("localStorage.setItem", "api.jwt", _jwt);
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);

                CurrentUser = new User { Email = email }; // minimal user info; optionally call /me to populate full profile
                OnAuthStateChanged?.Invoke();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SignUpAsync(string email, string password)
        {
            var payload = new { Email = email, Password = password };
            var resp = await _http.PostAsJsonAsync("/api/auth/register", payload);
            return resp.IsSuccessStatusCode;
        }

        public async Task SignOutAsync()
        {
            try { await _http.PostAsync("/api/auth/logout", null); } catch { }
            _jwt = null;
            CurrentUser = null;
            _http.DefaultRequestHeaders.Authorization = null;
            await _js.InvokeVoidAsync("localStorage.removeItem", "api.jwt");
            OnAuthStateChanged?.Invoke();
        }

        // --- Favorites API (adjust routes to match your backend if needed) ---
        public async Task<List<string>> GetFavoriteCitiesAsync()
        {
            if (!IsLoggedIn) return new List<string>();

            try
            {
                // expect backend returns ["City1","City2"] or [{ "city": "City1" }, ...]
                var list = await _http.GetFromJsonAsync<List<string>>("/api/favorites");
                if (list != null) return list;

                // fallback parse list of objects
                var raw = await _http.GetStringAsync("/api/favorites");
                var doc = JsonSerializer.Deserialize<JsonElement>(raw);
                var res = new List<string>();
                if (doc.ValueKind == JsonValueKind.Array)
                {
                    foreach (var it in doc.EnumerateArray())
                    {
                        if (it.ValueKind == JsonValueKind.String) res.Add(it.GetString()!);
                        else if (it.ValueKind == JsonValueKind.Object && it.TryGetProperty("city", out var c)) res.Add(c.GetString()!);
                    }
                }
                return res;
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<bool> AddFavoriteCityAsync(string city)
        {
            if (!IsLoggedIn || string.IsNullOrWhiteSpace(city)) return false;
            try
            {
                var resp = await _http.PostAsJsonAsync("/api/favorites", new { City = city });
                return resp.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> RemoveFavoriteCityAsync(string city)
        {
            if (!IsLoggedIn || string.IsNullOrWhiteSpace(city)) return false;
            try
            {
                // backend may require DELETE /api/favorites/{city}
                var encoded = Uri.EscapeDataString(city);
                var resp = await _http.DeleteAsync($"/api/favorites/{encoded}");
                return resp.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // minimal DTOs used locally
        public class User
        {
            public string? Id { get; set; }
            public string? Email { get; set; }
        }

        private class UserProfile
        {
            public string? Id { get; set; }
            public string? Email { get; set; }
        }
    }
}