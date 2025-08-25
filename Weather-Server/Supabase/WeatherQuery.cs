using Supabase;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Server.Supabase
{
    [Table("weather_queries")]
    public class WeatherQuery : BaseModel
    {
        [PrimaryKey("id", false)]
        public string? Id { get; set; }
        [Column("useremail")]
        public string? UserEmail { get; set; }
        [Column("city")]
        public string? City { get; set; }
        [Column("queriedat")]
        public DateTime QueriedAt { get; set; }
    }

    public partial class SupabaseUserService
    {
        public async Task LogWeatherQueryAsync(string? userEmail, string city)
        {
            var query = new WeatherQuery
            {
                UserEmail = userEmail,
                City = city,
                QueriedAt = DateTime.UtcNow
            };
            try
            {
                await _client.From<WeatherQuery>().Insert(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Supabase logging error: {ex.Message}\n{ex.StackTrace}");
                // Do not throw, just log
            }
        }
    }
}
