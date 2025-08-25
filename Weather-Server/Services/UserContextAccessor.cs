using System.Security.Claims;
using Server.Models;

namespace Server.Services
{
    public class UserContextAccessor : IUserContextAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserContext GetUserContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                           httpContext.User.FindFirst("sub")?.Value;
                var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value ??
                          httpContext.User.FindFirst("email")?.Value;

                return new UserContext
                {
                    UserId = userId ?? string.Empty,
                    Email = email ?? string.Empty,
                    IsAuthenticated = true
                };
            }

            return new UserContext { IsAuthenticated = false };
        }
    }
}
