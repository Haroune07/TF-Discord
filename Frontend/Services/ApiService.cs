using Shared.DTOs.Auth;
using System.Net.Http;
using System.Net.Http.Json;
using Shared.Constants;
using Shared.DTOs.Requests;

namespace Frontend.Services
{
    internal class ApiService
    {

        private readonly HttpClient _client;

        public ApiService()
        {
            _client = new();
            _client.BaseAddress = new Uri(Ports.SERVER_LISTEN_URL);
        }

        public async Task<AuthResponse> RegisterUserAsync(RegisterRequest req)
        {

            var res = await _client.PostAsJsonAsync(Routes.RegisterRoute, req);

            return (await res.Content.ReadFromJsonAsync<AuthResponse>())!;
        }

        public async Task<AuthResponse> LoginUserAsync(LoginRequest req)
        {
            var res = await _client.PostAsJsonAsync(Routes.LoginRoute, req);

            return (await res.Content.ReadFromJsonAsync<AuthResponse>())!;
        }

    }
}
