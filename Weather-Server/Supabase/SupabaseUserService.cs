using Supabase;
using System.Threading.Tasks;

namespace Server.Supabase
{
    public partial class SupabaseUserService
    {
        private readonly Client _client;

        public SupabaseUserService(string url, string apiKey)
        {
            _client = new Client(url, apiKey);
        }

        public async Task<bool> SignInAsync(string email, string password)
        {
            var session = await _client.Auth.SignIn(email, password);
            return session != null;
        }

        public async Task<bool> SignUpAsync(string email, string password)
        {
            var user = await _client.Auth.SignUp(email, password);
            return user != null;
        }

        public async Task SignOutAsync()
        {
            await _client.Auth.SignOut();
        }

        public bool IsAuthenticated => _client.Auth.CurrentUser != null;
    }
}
