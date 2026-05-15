using Backend.Src.Models;
using Backend.Src.Repository;
using Shared.DTOs;
using Shared.DTOs.Requests;
using Shared.Enums;

namespace Backend.Src.Services
{
    public class ChannelService
    {
        private readonly IRepository<Channel> _channels;
        private readonly IRepository<User> _users;
        private readonly IRepository<Server> _servers;
        private readonly IRepository<ServerMember> _members;
        private readonly MessageService _messageService;

        public ChannelService(
            IRepository<Channel> channelRepo,
            IRepository<User> userRepo,
            IRepository<Server> serverRepo,
            IRepository<ServerMember> memberRepo,
            MessageService messageService)
        {
            _channels = channelRepo;
            _users = userRepo;
            _servers = serverRepo;
            _members = memberRepo;
            _messageService = messageService;
        }

        public async Task<ChannelDTO?> CreateChannelAsync(string serverId, string requesterId, CreateChannelRequest req)
        {
            if (!await CanManageServerChannelsAsync(serverId, requesterId))
                return null;

            var server = await _servers.GetByIdAsync(serverId);
            if (server == null)
                return null;

            var channel = new Channel
            {
                Name = req.Name,
                ServerId = serverId,
                Type = ChannelType.Server,
                CreatedAt = DateTime.UtcNow
            };

            await _channels.InsertAsync(channel);

            return MapToDto(channel);
        }

        public async Task<bool> DeleteChannelAsync(string channelId, string requesterId)
        {
            var channel = await _channels.GetByIdAsync(channelId);
            if (channel == null || channel.Type != ChannelType.Server)
                return false;

            if (!await CanManageServerChannelsAsync(channel.ServerId, requesterId))
                return false;

            await _messageService.DeleteByChannelAsync(channelId);
            await _channels.DeleteAsync(channelId);
            return true;
        }

        public async Task<ChannelDTO> CreateDMChannelAsync(CreateDMRequest req)
        {
            var existingChannels = await _channels.FindAsync(c => c.Type == ChannelType.Direct && c.Participants != null && c.Participants.Contains(req.SenderId) && c.Participants.Contains(req.TargetUserId));
            var existing = existingChannels.FirstOrDefault();

            if (existing != null)
            {
                return new ChannelDTO
                {
                    Id = existing.Id,
                    Type = existing.Type,
                    CreatedAt = existing.CreatedAt,
                    Participants = existing.Participants
                        ?.Select(id => new UserDTO { Id = id })
                        .ToList()
                };
            }

            var channel = new Channel
            {
                Type = ChannelType.Direct,
                CreatedAt = DateTime.UtcNow,
                Participants = new List<string> { req.SenderId, req.TargetUserId }
            };

            await _channels.InsertAsync(channel);

            return new ChannelDTO
            {
                Id = channel.Id,
                Type = channel.Type,
                CreatedAt = channel.CreatedAt,
                Participants = channel.Participants?.Select(id => new UserDTO { Id = id }).ToList()
            };
        }

        public async Task<List<ChannelDTO>?> GetServerChannelsAsync(string serverId, string userId)
        {
            if (!await IsServerMemberAsync(serverId, userId))
                return null;

            var channels = await _channels.FindAsync(c => c.ServerId == serverId && c.Type == ChannelType.Server);

            return channels.Select(MapToDto).ToList();
        }

        public async Task<bool> CanAccessChannelAsync(string channelId, string userId)
        {
            if (string.IsNullOrWhiteSpace(channelId) || string.IsNullOrWhiteSpace(userId))
                return false;

            var channel = await _channels.GetByIdAsync(channelId);
            if (channel == null)
                return false;

            if (channel.Type == ChannelType.Direct)
                return channel.Participants?.Contains(userId) == true;

            if (channel.Type != ChannelType.Server || string.IsNullOrWhiteSpace(channel.ServerId))
                return false;

            return await IsServerMemberAsync(channel.ServerId, userId);
        }

        public async Task<bool> IsServerMemberAsync(string serverId, string userId)
        {
            if (string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(userId))
                return false;

            var members = await _members.FindAsync(m => m.ServerId == serverId && m.UserId == userId);
            return members.Any();
        }

        public async Task<List<ChannelDTO>> GetUserDMChannelsAsync(string userId)
        {
            var channels = await _channels.FindAsync(c => c.Type == ChannelType.Direct && c.Participants != null && c.Participants.Contains(userId));

            var results = new List<ChannelDTO>();
            foreach (var c in channels)
            {
                var participants = new List<UserDTO>();
                foreach (var pId in c.Participants ?? new List<string>())
                {
                    if (string.IsNullOrWhiteSpace(pId)) continue;

                    var user = await _users.GetByIdAsync(pId);
                    if (user != null)
                    {
                        participants.Add(new UserDTO
                        {
                            Id = user.Id,
                            Username = user.Username,
                            ProfileImageUrl = user.ProfileImageUrl,
                            IsOnline = user.IsOnline
                        });
                    }
                }

                results.Add(new ChannelDTO
                {
                    Id = c.Id,
                    Type = c.Type,
                    CreatedAt = c.CreatedAt,
                    Participants = participants
                });
            }
            return results;
        }

        private async Task<bool> CanManageServerChannelsAsync(string serverId, string requesterId)
        {
            if (string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(requesterId))
                return false;

            var members = await _members.FindAsync(m => m.ServerId == serverId && m.UserId == requesterId);
            var member = members.FirstOrDefault();

            return member is not null &&
                   (member.Role == MemberRole.Owner || member.Role == MemberRole.Admin);
        }

        private static ChannelDTO MapToDto(Channel channel) => new()
        {
            Id = channel.Id,
            Name = channel.Name,
            ServerId = channel.ServerId,
            Type = channel.Type,
            CreatedAt = channel.CreatedAt
        };
    }
}
