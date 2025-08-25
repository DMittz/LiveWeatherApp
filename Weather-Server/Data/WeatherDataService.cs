using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Data
{
    public class WeatherDataService
    {
        private readonly IMongoCollection<WeatherData> _weatherCollection;

        public WeatherDataService(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _weatherCollection = database.GetCollection<WeatherData>(collectionName);
        }

        public async Task<List<WeatherData>> GetWeatherDataByUserAsync(string userId)
        {
            var filter = Builders<WeatherData>.Filter.Eq(w => w.UserId, userId);
            return await _weatherCollection.Find(filter).ToListAsync();
        }

        public async Task<WeatherData?> GetWeatherDataByCityAsync(string userId, string city)
        {
            var filter = Builders<WeatherData>.Filter.And(
                Builders<WeatherData>.Filter.Eq(w => w.UserId, userId),
                Builders<WeatherData>.Filter.Eq(w => w.City, city)
            );
            return await _weatherCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task SaveWeatherDataAsync(WeatherData weatherData)
        {
            var filter = Builders<WeatherData>.Filter.And(
                Builders<WeatherData>.Filter.Eq(w => w.UserId, weatherData.UserId),
                Builders<WeatherData>.Filter.Eq(w => w.City, weatherData.City)
            );

            var existing = await _weatherCollection.Find(filter).FirstOrDefaultAsync();
            if (existing == null)
            {
                await _weatherCollection.InsertOneAsync(weatherData);
            }
            else
            {
                await _weatherCollection.ReplaceOneAsync(filter, weatherData);
            }
        }
    }

    public class WeatherData
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public double Temperature { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public int Humidity { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
