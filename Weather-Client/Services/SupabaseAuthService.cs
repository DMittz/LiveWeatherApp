using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Supabase;
using Supabase.Gotrue;
using Supabase.Postgrest;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace Client.Services
{
    public class SupabaseAuthService
    {
        private Supabase.Client? _supabaseClient;
        private readonly IJSRuntime _js;
        private readonly IConfiguration _configuration;
        private readonly string? _supabaseUrl;
        private readonly string? _supabaseKey;

        public User? CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;
        public string? UserEmail => CurrentUser?.Email;
        public event Action? OnAuthStateChanged;

        public string? GetJwt()
        {
            return _supabaseClient?.Auth?.CurrentSession?.AccessToken;
        }

        public SupabaseAuthService(IJSRuntime jsRuntime, IConfiguration configuration)
        {
            _js = jsRuntime;
            _configuration = configuration;

            // Read config but don't throw here; initialization happens in InitializeAsync.
            _supabaseUrl = _configuration["Supabase:Url"];
            _supabaseKey = _configuration["Supabase:Key"];
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(_supabaseUrl) && !string.IsNullOrEmpty(_supabaseKey))
                {
                    Console.WriteLine($"Initializing Supabase client with URL: {_supabaseUrl} and Key: {_supabaseKey}");
                    _supabaseClient = new Supabase.Client(_supabaseUrl, _supabaseKey);
                    Console.WriteLine("Supabase client initialized successfully.");
                    await _supabaseClient.InitializeAsync();
                }

                var json = await _js.InvokeAsync<string>("localStorage.getItem", "supabase.session");
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        var data = JsonSerializer.Deserialize<SessionData>(json);
                        if (data != null && !string.IsNullOrEmpty(data.AccessToken) && !string.IsNullOrEmpty(data.RefreshToken))
                        {
                            if (_supabaseClient != null)
                            {
                                await _supabaseClient.Auth.SetSession(data.AccessToken, data.RefreshToken);
                                CurrentUser = _supabaseClient.Auth.CurrentUser;
                                OnAuthStateChanged?.Invoke();
                            }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                // log and continue so the app UI doesn't crash
                Console.Error.WriteLine($"Supabase init error: {ex.Message}");
            }
        }

        public async Task<bool> SignInAsync(string email, string password)
        {
            if (_supabaseClient == null)
            {
                Console.WriteLine("Supabase client is not initialized.");
                return false;
            }
            Console.WriteLine($"Attempting to sign in with email: {email}");
            var result = await _supabaseClient.Auth.SignIn(email, password);
            if (result != null && result.User != null)
            {
                CurrentUser = result.User;
                var sessionData = new SessionData
                {
                    AccessToken = result.AccessToken ?? "",
                    RefreshToken = result.RefreshToken ?? ""
                };
                var sessionJson = JsonSerializer.Serialize(sessionData);
                await _js.InvokeVoidAsync("localStorage.setItem", "supabase.session", sessionJson);
                OnAuthStateChanged?.Invoke();
                return true;
            }
            return false;
        }

        public async Task<bool> SignUpAsync(string email, string password)
        {
            if (_supabaseClient == null)
            {
                Console.WriteLine("Supabase client is not initialized.");
                return false;
            }
            Console.WriteLine($"Attempting to sign up with email: {email}");
            var result = await _supabaseClient.Auth.SignUp(email, password);
            if (result.User != null)
            {
                CurrentUser = result.User;
                OnAuthStateChanged?.Invoke();
                return true;
            }
            return false;
        }

        public async Task SignOutAsync()
        {
            await _supabaseClient.Auth.SignOut();
            CurrentUser = null;
            await _js.InvokeVoidAsync("localStorage.removeItem", "supabase.session");
            OnAuthStateChanged?.Invoke();
        }

        // ---- FAVORITES ----

        public async Task<List<string>> GetFavoriteCitiesAsync()
        {
            if (CurrentUser == null) return new List<string>();

            try
            {
                var result = await _supabaseClient
                    .From<WeatherFavorite>()
                    .Where(x => x.UserId == CurrentUser.Id)
                    .Get();

                return result.Models.Select(f => f.City).ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<bool> AddFavoriteCityAsync(string city)
        {
            if (CurrentUser == null || string.IsNullOrEmpty(city)) return false;

            try
            {
                var newFavorite = new WeatherFavorite { UserId = CurrentUser.Id, City = city };
                await _supabaseClient.From<WeatherFavorite>().Insert(newFavorite);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveFavoriteCityAsync(string city)
        {
            if (CurrentUser == null || string.IsNullOrEmpty(city)) return false;

            try
            {
                await _supabaseClient
                    .From<WeatherFavorite>()
                    .Where(x => x.UserId == CurrentUser.Id && x.City == city)
                    .Delete();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private class SessionData
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = "";

            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; set; } = "";
        }

        public class WeatherFavorite : Supabase.Postgrest.Models.BaseModel
        {
            [JsonPropertyName("user_id")]
            public string UserId { get; set; } = "";

            [JsonPropertyName("city")]
            public string City { get; set; } = "";
        }
    }
}
