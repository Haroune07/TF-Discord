using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.DTOs;
using Shared.DTOs.Auth;
using Shared.DTOs.Requests;

namespace Frontend.Services
{
    public interface IApiService
    {
        Task<AuthResponse> RegisterUserAsync(RegisterRequest req);
        Task<AuthResponse> LoginUserAsync(LoginRequest req);
        Task<List<ChannelDTO>> GetServerChannelsAsync(string serverId, string userId);
        Task<ChannelDTO?> CreateServerChannelAsync(string requesterId, CreateChannelRequest req);
        Task<bool> DeleteChannelAsync(string channelId, string requesterId);
        Task<ChannelDTO?> CreateDMAsync(CreateDMRequest req);
        Task<List<MessageDTO>> GetMessagesAsync(string channelId, string userId);
        Task<MessageDTO?> SendMessageAsync(CreateMessageRequest req);
        Task<MessageDTO?> EditMessageAsync(string messageId, EditMessageRequest req);
        Task<bool> DeleteMessageAsync(string messageId, string requesterId);
        Task<List<ServerDTO>> GetMyServersAsync(string userId);
        Task<ServerDTO?> CreateServerAsync(CreateServerRequest req);
        Task<bool> JoinServerAsync(JoinOrLeaveServerRequest req);
        Task<bool> LeaveServerAsync(JoinOrLeaveServerRequest req);
        Task<List<ServerDTO>> GetAllServersAsync();
        Task<List<UserDTO>> GetAllUsersExceptMeAsync(string userId);
        Task<bool> UpdateStatusAsync(string userId, string status);
        Task<bool> UploadProfileImageAsync(string userId, string imageUrl);
        Task<bool> SendFriendRequestAsync(string requesterId, string targetUsername);
        Task<List<FriendshipDTO>> GetFriendsAsync(string userId);
        Task<List<FriendshipDTO>> GetPendingRequestsAsync(string userId);
        Task<bool> UpdateFriendshipStatusAsync(string friendshipId, string userId, Shared.Enums.FriendshipStatus status);
        Task<List<ChannelDTO>> GetMyDMChannelsAsync(string userId);
        Task<List<UserDTO>> SearchUsersAsync(string username);
        Task<List<ServerMemberDTO>> GetServerMembersAsync(string serverId);
        Task<bool> KickMemberAsync(string serverId, string requesterId, string targetUserId);
    }
}
