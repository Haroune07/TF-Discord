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

        public ChannelService(IRepository<Channel> channelRepo, IRepository<User> userRepo, IRepository<Server> serverRepo)
        {
            _channels = channelRepo;
            _users = userRepo;
            _servers = serverRepo;
        }

        public async Task<ChannelDTO> CreateServerChannelAsync(CreateChannelRequest req)
        {
            var server = await _servers.GetByIdAsync(req.ServerId);
            if (server == null)
                throw new Exception("Server not found");

            var channel = new Channel
            {
                Name = req.Name,
                ServerId = req.ServerId,
                Type = ChannelType.Server,
                CreatedAt = DateTime.UtcNow
            };

            await _channels.InsertAsync(channel);

            return new ChannelDTO
            {
                Id = channel.Id,
                Name = channel.Name,
                ServerId = channel.ServerId,
                Type = channel.Type,
                CreatedAt = channel.CreatedAt,
            };
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

        public async Task<List<ChannelDTO>> GetServerChannelsAsync(string serverId)
        {
            var channels = await _channels.FindAsync(c => c.ServerId == serverId && c.Type == ChannelType.Server);

            return channels.Select(c => new ChannelDTO
            {
                Id = c.Id,
                Name = c.Name,
                ServerId = c.ServerId,
                Type = c.Type,
                CreatedAt = c.CreatedAt
            }).ToList();
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
    }
}
