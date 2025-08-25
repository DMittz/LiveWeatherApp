using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Server.Data;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/forecast")]
    public class ForecastController : ControllerBase
    {
        private readonly ForecastService _forecastService;

        public ForecastController(ForecastService forecastService)
        {
            _forecastService = forecastService;
        }

        [HttpGet("city/{city}")]
        public async Task<IActionResult> GetForecastByCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("City name is required.");

            try
            {
                var result = await _forecastService.GetForecastByCityAsync(city);
                if (result == null)
                    return NotFound(new { message = "Weather forecast not found for the specified city." });

                return Ok(result);
            }
            catch (OpenWeatherMapException ex)
            {
                return StatusCode((int)ex.StatusCode, new { message = ex.Message, error = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the weather forecast.", details = ex.Message });
            }
        }

        [HttpGet("coords")]
        public async Task<IActionResult> GetForecastByCoordinates([FromQuery] double lat, [FromQuery] double lon)
        {
            if (lat < -90 || lat > 90)
                return BadRequest("Latitude must be between -90 and 90.");
            
            if (lon < -180 || lon > 180)
                return BadRequest("Longitude must be between -180 and 180.");

            try
            {
                var result = await _forecastService.GetForecastByCoordinatesAsync(lat, lon);
                if (result == null)
                    return NotFound(new { message = "Weather forecast not found for the specified coordinates." });

                return Ok(result);
            }
            catch (OpenWeatherMapException ex)
            {
                return StatusCode((int)ex.StatusCode, new { message = ex.Message, error = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the weather forecast.", details = ex.Message });
            }
        }
    }
}
