using Backend.Src.Models;
using Backend.Src.Repository;
using Shared.DTOs.Requests;
using Shared.Enums;
using Shared.DTOs;

namespace Backend.Src.Services
{
    public class ChannelService
    {
        private readonly IRepository<Channel> _channels;

        public ChannelService(IRepository<Channel> channelRepo)
        {
            _channels = channelRepo;
        }

        public async Task<ChannelDTO> CreateServerChannelAsync(CreateChannelRequest req)
        {
            var channel = new Channel
            {
                Name = req.Name,
                ServerId = req.ServerId,
                Type = ChannelType.Server,
                CreatedAt = DateTime.Now
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
                CreatedAt = DateTime.Now,
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
    }
}
