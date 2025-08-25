namespace Server.Models
{
    public class AuthMeResponse
    {
        public bool IsAuthenticated { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
    }

    public class FavoriteRequest
    {
        public string CityName { get; set; } = string.Empty;
    }
}
