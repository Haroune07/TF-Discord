using Shared.Constants;
using Shared.DTOs;
using Shared.DTOs.Auth;
using Shared.DTOs.Requests;
using System.Net.Http;
using System.Net.Http.Json;

namespace Frontend.Services
{
    public class ApiService
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

        public async Task<MessageDTO?> EditMessageAsync(string messageId, EditMessageRequest req)
        {
            var res = await _client.PatchAsJsonAsync($"{Routes.EditMessageRoute}/{messageId}", req);
            return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<MessageDTO>() : null;
        }

        public async Task<bool> DeleteMessageAsync(string messageId, string requesterId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{Routes.DeleteMessageRoute}/{messageId}")
            {
                Content = JsonContent.Create(new DeleteMessageRequest { RequesterId = requesterId })
            };
            var res = await _client.SendAsync(request);
            return res.IsSuccessStatusCode;
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

        public async Task<bool> JoinServerAsync(JoinOrLeaveServerRequest req)
        {
            var res = await _client.PostAsJsonAsync(Routes.JoinServerRoute, req);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> LeaveServerAsync(JoinOrLeaveServerRequest req)
        {
            var res = await _client.PostAsJsonAsync(Routes.LeaveServerRoute, req);
            return res.IsSuccessStatusCode;
        }

        public async Task<List<ServerDTO>> GetAllServersAsync()
        {
            var res = await _client.GetFromJsonAsync<List<ServerDTO>>(Routes.GetAllServersRoute);

            return res ?? new();
        }

        public async Task<List<UserDTO>> GetAllUsersExceptMeAsync(string userId)
        {
            return await _client.GetFromJsonAsync<List<UserDTO>>($"{Routes.GetAllUsersRoute}/{userId}") ?? new();
        }

        public async Task<bool> UpdateStatusAsync(string userId, string status)
        {
            var url = string.Format(Routes.UpdateStatus, userId);
            var response = await _client.PutAsJsonAsync(url, status);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UploadProfileImageAsync(string userId, string imageUrl)
        {
            var route = string.Format(Routes.UpdatePfp, userId);
            var response = await _client.PutAsJsonAsync(route, imageUrl);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendFriendRequestAsync(string requesterId, string targetUsername)
        {
            var url = $"{Routes.SendFriendRequestRoute}?requesterId={requesterId}&targetUsername={targetUsername}";
            var response = await _client.PostAsync(url, null);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<FriendshipDTO>> GetFriendsAsync(string userId)
        {
            return await _client.GetFromJsonAsync<List<FriendshipDTO>>($"{Routes.GetFriendsRoute}/{userId}") ?? new();
        }

        public async Task<List<FriendshipDTO>> GetPendingRequestsAsync(string userId)
        {
            return await _client.GetFromJsonAsync<List<FriendshipDTO>>($"{Routes.GetPendingFriendsRoute}/{userId}") ?? new();
        }

        public async Task<bool> UpdateFriendshipStatusAsync(string friendshipId, string userId, Shared.Enums.FriendshipStatus status)
        {
            var url = $"{Routes.UpdateFriendshipStatusRoute}/{friendshipId}/status?userId={userId}&status={status}";
            var response = await _client.PutAsync(url, null);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ChannelDTO>> GetMyDMChannelsAsync(string userId)
        {
            return await _client.GetFromJsonAsync<List<ChannelDTO>>($"/api/channel/dm/{userId}") ?? new();
        }

        public async Task<List<UserDTO>> SearchUsersAsync(string username)
        {
            return await _client.GetFromJsonAsync<List<UserDTO>>($"{Routes.SearchUser}/{username}") ?? new();
        }

    }
}