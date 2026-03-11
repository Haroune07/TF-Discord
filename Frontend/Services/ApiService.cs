using Shared.DTOs.Auth;
using System.Net.Http;
using System.Net.Http.Json;

namespace Frontend.Services
{
    internal class ApiService
    {

        private readonly HttpClient _client;

        public ApiService(string baseUrl)
        {
            _client = new()
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        async Task<AuthResponse> PostRegisterResponse(RegisterRequest req)
        {

            var res = await _client.PostAsJsonAsync("api/auth/register", req);
            return (await res.Content.ReadFromJsonAsync<AuthResponse>())!;
        }

    }
}
