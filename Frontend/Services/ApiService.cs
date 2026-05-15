using Shared.Constants;
using Shared.DTOs;
using Shared.DTOs.Auth;
using Shared.DTOs.Requests;
using System.Net.Http;
using System.Net.Http.Json;

namespace Frontend.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _client;
        private Action<string>? _errorCallback;

        public ApiService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(Ports.SERVER_LISTEN_URL),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public void SetErrorCallback(Action<string>? callback) => _errorCallback = callback;

        private async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception)
            {
                _errorCallback?.Invoke("Impossible de joindre le serveur. Vérifiez votre connexion.");
                throw;
            }
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

        public async Task<List<ChannelDTO>> GetServerChannelsAsync(string serverId, string userId)
        {
            var response = await _client.GetAsync($"{Routes.GetServerChannelsRoute}/{serverId}?userId={userId}");
            if (!response.IsSuccessStatusCode)
                return new();

            return await response.Content.ReadFromJsonAsync<List<ChannelDTO>>() ?? new();
        }

        public async Task<ChannelDTO?> CreateServerChannelAsync(string requesterId, CreateChannelRequest req)
        {
            var res = await _client.PostAsJsonAsync(
                $"{Routes.CreateServerChannelRoute}?requesterId={requesterId}", req);
            return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<ChannelDTO>() : null;
        }

        public async Task<bool> DeleteChannelAsync(string channelId, string requesterId)
        {
            var res = await _client.DeleteAsync(
                $"{Routes.DeleteChannelRoute}/{channelId}?requesterId={requesterId}");
            return res.IsSuccessStatusCode;
        }

        public async Task<ChannelDTO?> CreateDMAsync(CreateDMRequest req)
        {
            var res = await _client.PostAsJsonAsync(Routes.CreateDMChannelRoute, req);
            return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<ChannelDTO>() : null;
        }

        public async Task<List<MessageDTO>> GetMessagesAsync(string channelId, string userId) =>
            await ExecuteAsync(async () =>
            {
                var response = await _client.GetAsync($"{Routes.GetChannelMessagesRoute}/channel/{channelId}?userId={userId}");
                if (!response.IsSuccessStatusCode)
                    return new List<MessageDTO>();

                return await response.Content.ReadFromJsonAsync<List<MessageDTO>>() ?? new();
            });

        public async Task<MessageDTO?> SendMessageAsync(CreateMessageRequest req) =>
            await ExecuteAsync(async () =>
            {
                var res = await _client.PostAsJsonAsync(Routes.SendMessageRoute, req);
                return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<MessageDTO>() : null;
            });

        public async Task<MessageDTO?> EditMessageAsync(string messageId, EditMessageRequest req) =>
            await ExecuteAsync(async () =>
            {
                var res = await _client.PatchAsJsonAsync($"{Routes.EditMessageRoute}/{messageId}", req);
                return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<MessageDTO>() : null;
            });

        public async Task<bool> DeleteMessageAsync(string messageId, string requesterId) =>
            await ExecuteAsync(async () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, $"{Routes.DeleteMessageRoute}/{messageId}")
                {
                    Content = JsonContent.Create(new DeleteMessageRequest { RequesterId = requesterId })
                };
                var res = await _client.SendAsync(request);
                return res.IsSuccessStatusCode;
            });

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
            var isOnline = status.Equals("True", StringComparison.OrdinalIgnoreCase)
                || status.Equals("online", StringComparison.OrdinalIgnoreCase);
            var url = $"/api/user/{userId}/status?isOnline={isOnline}";
            var response = await _client.PatchAsync(url, null);
            return response.IsSuccessStatusCode;
        }

        public Task<bool> SetOnlineAsync(string userId, bool isOnline) =>
            UpdateStatusAsync(userId, isOnline ? "True" : "False");

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

        public async Task<List<ServerMemberDTO>> GetServerMembersAsync(string serverId) =>
            await _client.GetFromJsonAsync<List<ServerMemberDTO>>($"/api/server/{serverId}/members") ?? new();

        public async Task<bool> KickMemberAsync(string serverId, string requesterId, string targetUserId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/server/members/kick")
            {
                Content = JsonContent.Create(new KickMemberRequest
                {
                    ServerId = serverId,
                    RequesterId = requesterId,
                    TargetUserId = targetUserId
                })
            };
            var response = await _client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
