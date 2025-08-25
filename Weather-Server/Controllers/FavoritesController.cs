using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Supabase;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly SupabaseFavoritesService _favoritesService;

        public FavoritesController(SupabaseFavoritesService favoritesService)
        {
            _favoritesService = favoritesService;
        }

        // POST /api/favorites
        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] FavoriteCityRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("User ID not found in claims.");
            }

            if (string.IsNullOrWhiteSpace(request.City))
            {
                return BadRequest("City name is required.");
            }

            await _favoritesService.AddFavoriteCityAsync(userId, request.City);
            return Ok(new { message = $"City '{request.City}' added to favorites." });
        }

    // GET /api/favorites
    [HttpGet]
    public async Task<IActionResult> GetFavorites()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("User ID not found in claims.");
        }

        var favoriteCities = await _favoritesService.GetFavoriteCitiesAsync(userId);
        return Ok(favoriteCities);
    }

    // DELETE /api/favorites/{city}
    [HttpDelete("{city}")]
    public async Task<IActionResult> DeleteFavorite(string city)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("User ID not found in claims.");
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return BadRequest("City name is required.");
        }

        await _favoritesService.RemoveFavoriteCityAsync(userId, city);
        return Ok(new { message = $"City '{city}' removed from favorites." });
    }
    }

    public class FavoriteCityRequest
    {
        public string City { get; set; } = string.Empty;
    }
}
