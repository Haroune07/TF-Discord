using Shared.Constants;
using Shared.DTOs;
using Shared.DTOs.Auth;
using Shared.DTOs.Requests;
using System.Net.Http;
using System.Net.Http.Json;

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

        public async Task<List<ChannelDTO>> GetServerChannelsAsync(string serverId)
        {
            return await _client.GetFromJsonAsync<List<ChannelDTO>>($"{Routes.GetServerChannelsRoute}/{serverId}") ?? new();
        }

        public async Task<ChannelDTO?> CreateDMAsync(CreateDMRequest req)
        {
            var res = await _client.PostAsJsonAsync(Routes.CreateDMChannelRoute, req);
            return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<ChannelDTO>() : null;
        }

        public async Task<List<MessageDTO>> GetMessagesAsync(string channelId)
        {
            return await _client.GetFromJsonAsync<List<MessageDTO>>($"{Routes.GetChannelMessagesRoute}/channel/{channelId}") ?? new();
        }

        public async Task<MessageDTO?> SendMessageAsync(CreateMessageRequest req)
        {
            var res = await _client.PostAsJsonAsync(Routes.SendMessageRoute, req);
            return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<MessageDTO>() : null;
        }

        public async Task<List<ServerDTO>> GetMyServersAsync(string userId)
        {
            return await _client.GetFromJsonAsync<List<ServerDTO>>($"{Routes.GetMyServersRoute}/{userId}") ?? new();
        }

        public async Task<ServerDTO?> CreateServerAsync(CreateServerRequest req)
        {
            var res = await _client.PostAsJsonAsync(Routes.CreateServerRoute, req);
            return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<ServerDTO>() : null;
        }

        public async Task<bool> JoinServerAsync(JoinServerRequest req)
        {
            var res = await _client.PostAsJsonAsync(Routes.JoinServerRoute, req);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> LeaveServerAsync(JoinServerRequest req)
        {
            var res = await _client.PostAsJsonAsync(Routes.LeaveServerRoute, req);
            return res.IsSuccessStatusCode;
        }

    }
}