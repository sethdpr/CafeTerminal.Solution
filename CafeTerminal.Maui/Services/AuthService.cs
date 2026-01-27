using CafeTerminal.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CafeTerminal.Maui.Services
{
    public class AuthService
    {
        private readonly HttpClient _client;

        public AuthService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://10.0.2.2:5001/api/")
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var response = await _client.PostAsJsonAsync("auth/login", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
            var response = await _client.PostAsJsonAsync("auth/register", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task AddAuthHeaderAsync()
        {
            var token = await SecureStorage.GetAsync("auth_token");

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
