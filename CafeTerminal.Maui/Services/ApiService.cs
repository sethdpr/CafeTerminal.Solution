using System.Net.Http.Headers;

namespace CafeTerminal.Maui.Services
{
    public class ApiService /*This service bundles some functionality:
                             1. Gets the API adress
                             2. Determines what HttpClient gets used
                             3. Gets JWT-token from SecureStorage
                             4. Adds authorization header to give to API for authentication
                             5. Excecutes wanted Http-request*/
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7232")
            };
        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            var token = await SecureStorage.Default.GetAsync("auth_token");

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await _httpClient.GetAsync(endpoint);
        }
    }
}