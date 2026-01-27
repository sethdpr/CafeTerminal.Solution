using System.Net.Http.Headers;

namespace CafeTerminal.Api.Services
{
    public class ProfileService
    {
        private readonly HttpClient _client;

        public ProfileService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://10.0.2.2:5001/api/")
            };
        }

        public async Task<string> GetProfileAsync()
        {
            var token = await SecureStorage.GetAsync("auth_token");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("profile");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}