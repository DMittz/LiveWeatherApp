using Server.Models;

namespace Server.Services
{
    public interface IUserContextAccessor
    {
        UserContext GetUserContext();
    }
}
