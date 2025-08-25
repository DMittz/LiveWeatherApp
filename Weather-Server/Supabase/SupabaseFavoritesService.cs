using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Supabase
{
    [Table("favorites")]
    public class Favorite : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid id { get; set; }
        public Guid user_id { get; set; }
        public string city_name { get; set; } = string.Empty;
    }

    public class SupabaseFavoritesService
    {
        private readonly Client _client;

        public SupabaseFavoritesService(string url, string apiKey)
        {
            _client = new Client(url, apiKey);
            _client.InitializeAsync().GetAwaiter().GetResult();
        }

        public async Task<List<string>> GetFavoriteCitiesAsync(Guid userId)
        {
            try
            {
                var favorites = await _client
                    .From<Favorite>()
                    .Where(f => f.user_id == userId)
                    .Get();

                var cityNames = new List<string>();
                foreach (var fav in favorites.Models)
                {
                    cityNames.Add(fav.city_name);
                }
                return cityNames;
            }
            catch (Exception ex)
            {
                // Log or handle error
                throw new Exception("Failed to get favorite cities", ex);
            }
        }

    public async Task AddFavoriteCityAsync(Guid userId, string cityName)
    {
        try
        {
            // Check if city already favorited
            var existing = await _client
                .From<Favorite>()
                .Where(f => f.user_id == userId && f.city_name == cityName)
                .Get();

            if (existing.Models.Count > 0)
            {
                // Already favorited, do nothing or throw
                return;
            }

            var favorite = new Favorite
            {
                id = Guid.NewGuid(),
                user_id = userId,
                city_name = cityName
            };

            await _client.From<Favorite>().Insert(favorite);
        }
        catch (Exception ex)
        {
            // Log or handle error
            throw new Exception("Failed to add favorite city", ex);
        }
    }

    public async Task RemoveFavoriteCityAsync(Guid userId, string cityName)
    {
        try
        {
            var existing = await _client
                .From<Favorite>()
                .Where(f => f.user_id == userId && f.city_name == cityName)
                .Get();

            if (existing.Models.Count == 0)
            {
                // City not found in favorites
                return;
            }

            var favorite = existing.Models.First();
            await _client.From<Favorite>().Delete(favorite);
        }
        catch (Exception ex)
        {
            // Log or handle error
            throw new Exception("Failed to remove favorite city", ex);
        }
    }
    }
}
