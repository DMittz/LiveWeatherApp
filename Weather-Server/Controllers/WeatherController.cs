using Microsoft.AspNetCore.Mvc;
using Server.Data;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly OpenWeatherMapService _openWeather;

    public WeatherController(OpenWeatherMapService openWeather)
    {
        _openWeather = openWeather;
    }

    // GET api/weather?city=...  OR  GET api/weather?lat=12.9&lon=72
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? city, [FromQuery] double? lat, [FromQuery] double? lon)
    {
        if (lat.HasValue && lon.HasValue)
        {
            var current = await _openWeather.GetWeatherByCoordinatesAsync(lat.Value, lon.Value);
            if (current == null) return NotFound();

            return Ok(MapCurrentWeather(current));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            var current = await _openWeather.GetWeatherByCityAsync(city);
            if (current == null) return NotFound();

            return Ok(MapCurrentWeather(current));
        }

        return BadRequest("Provide either city or both lat and lon.");
    }

    private object MapCurrentWeather(OpenWeatherResponse current)
    {
        var weather = current.Weather?.FirstOrDefault();
        return new
        {
            City = current.Name ?? "",
            Temp = current.Main?.Temp,
            TempMin = current.Main?.Temp_Min,
            TempMax = current.Main?.Temp_Max,
            Humidity = current.Main?.Humidity,
            Condition = weather?.Main,
            Description = weather?.Description,
            Icon = weather?.Icon
        };
    }
}
