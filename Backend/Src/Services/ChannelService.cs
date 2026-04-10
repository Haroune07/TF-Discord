using Backend.Src.Models;
using Backend.Src.Repository;
using Shared.DTOs.Requests;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Src.Services
{
    public class ChannelService
    {
        private readonly IRepository<Channel> _channels;

        public ChannelService(IRepository<Channel> channelRepo)
        {
            _channels = channelRepo;
        }

        public async Task<Channel> CreateServerChannelAsync(CreateChannelRequest req)
        {
            var channel = new Channel
            {
                Name = req.Name,
                ServerId = req.ServerId,
                Type = ChannelType.Server,
                CreatedAt = DateTime.Now
            };

            await _channels.InsertAsync(channel);
            return channel;
        }

        public async Task<Channel> CreateDMChannelAsync(CreateDMRequest req)
        {
            var existingChannels = await _channels.FindAsync(c => c.Type == ChannelType.Direct && c.Participants != null && c.Participants.Contains(req.SenderId) && c.Participants.Contains(req.TargetUserId));
            var existing = existingChannels.FirstOrDefault();

            if (existing != null)
            {
                return existing;
            }

            var channel = new Channel
            {
                Type = ChannelType.Direct,
                CreatedAt = DateTime.Now,
                Participants = new List<string> { req.SenderId, req.TargetUserId }
            };

            await _channels.InsertAsync(channel);
            return channel;
        }
    }
}
