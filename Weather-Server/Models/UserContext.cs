namespace Server.Models
{
    public class UserContext
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
    }
}
