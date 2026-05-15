using Backend.Src.Mappers;
using Backend.Src.Models;
using Backend.Src.Repository;
using Shared.DTOs;
using Shared.DTOs.Requests;
using Shared.Enums;

namespace Backend.Src.Services
{
    public class MessageService
    {
        private readonly IRepository<Message> _messages;
        private readonly IRepository<User> _users;
        private readonly IRepository<Channel> _channels;
        private readonly IRepository<ServerMember> _members;

        public MessageService(
            IRepository<Message> messageRepo,
            IRepository<User> userRepo,
            IRepository<Channel> channelRepo,
            IRepository<ServerMember> memberRepo)
        {
            _messages = messageRepo;
            _users = userRepo;
            _channels = channelRepo;
            _members = memberRepo;
        }

        public async Task<MessageDTO?> SendMessageAsync(CreateMessageRequest req)
        {
            if (!await CanAccessChannelAsync(req.ChannelId, req.SenderId))
                return null;

            var sender = await _users.GetByIdAsync(req.SenderId);
            if (sender == null)
                return null;

            var message = new Message
            {
                Content = req.Content,
                ChannelId = req.ChannelId,
                SenderId = req.SenderId,
                SentAt = DateTime.UtcNow
            };

            await _messages.InsertAsync(message);

            return new MessageDTO
            {
                Id = message.Id,
                Content = message.Content,
                ChannelId = message.ChannelId,
                SentAt = message.SentAt,
                Sender = sender.ToDTO()!
            };
        }

        public async Task<List<MessageDTO>?> GetMessagesByChannelAsync(string channelId, string userId)
        {
            if (!await CanAccessChannelAsync(channelId, userId))
                return null;

            var messages = await _messages.FindAsync(m => m.ChannelId == channelId);

            var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();
            var senders = await _users.FindAsync(u => senderIds.Contains(u.Id));
            var senderMap = senders.ToDictionary(u => u.Id);

            return messages
                .OrderBy(m => m.SentAt)
                .Select(m =>
                {
                    senderMap.TryGetValue(m.SenderId, out var sender);
                    return new MessageDTO
                    {
                        Id = m.Id,
                        Content = m.Content,
                        ChannelId = m.ChannelId,
                        SentAt = m.SentAt,
                        Sender = sender == null ? new UserDTO { Id = m.SenderId } : sender.ToDTO()!
                    };
                })
                .ToList();
        }

        public async Task<MessageDTO?> EditMessageAsync(string messageId, string requesterId, string newContent)
        {
            var message = await _messages.GetByIdAsync(messageId);
            if (message == null || message.SenderId != requesterId) return null;

            message.Content = newContent;
            await _messages.UpdateAsync(messageId, message);

            var sender = await _users.GetByIdAsync(message.SenderId);
            return new MessageDTO
            {
                Id = message.Id,
                Content = message.Content,
                ChannelId = message.ChannelId,
                SentAt = message.SentAt,
                Sender = sender == null ? new UserDTO { Id = message.SenderId } : sender.ToDTO()!,
                IsEdited = true
            };
        }

        public async Task<bool> DeleteMessageAsync(string messageId, string requesterId)
        {
            var message = await _messages.GetByIdAsync(messageId);
            if (message == null || message.SenderId != requesterId) return false;

            await _messages.DeleteAsync(messageId);
            return true;
        }

        public async Task DeleteByChannelAsync(string channelId)
        {
            var messages = await _messages.FindAsync(m => m.ChannelId == channelId);
            foreach (var message in messages)
                await _messages.DeleteAsync(message.Id);
        }

        private async Task<bool> CanAccessChannelAsync(string channelId, string userId)
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

            var members = await _members.FindAsync(m =>
                m.ServerId == channel.ServerId && m.UserId == userId);

            return members.Any();
        }
    }
}
