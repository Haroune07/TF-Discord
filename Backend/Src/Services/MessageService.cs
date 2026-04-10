using Backend.Src.Models;
using Backend.Src.Repository;
using Shared.DTOs.Requests;
using Shared.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Src.Services
{
    public class MessageService
    {
        private readonly IRepository<Message> _messages;
        private readonly IRepository<User> _users;

        public MessageService(IRepository<Message> messageRepo, IRepository<User> userRepo)
        {
            _messages = messageRepo;
            _users = userRepo;
        }

        public async Task<MessageDTO> SendMessageAsync(CreateMessageRequest req)
        {
            var message = new Message
            {
                Content = req.Content,
                ChannelId = req.ChannelId,
                SenderId = req.SenderId,
                SentAt = DateTime.Now
            };

            await _messages.InsertAsync(message);

            var sender = await _users.GetByIdAsync(message.SenderId);

            return new MessageDTO
            {
                Id = message.Id,
                Content = message.Content,
                ChannelId = message.ChannelId,
                SentAt = message.SentAt,
                Sender = sender == null ? new UserDTO { Id = message.SenderId } : new UserDTO
                {
                    Id = sender.Id,
                    Username = sender.Username,
                    IsOnline = sender.IsOnline,
                    ProfileImageUrl = sender.ProfileImageUrl,
                    CreatedAt = sender.CreatedAt
                }
            };
        }

        public async Task<List<MessageDTO>> GetMessagesByChannelAsync(string channelId)
        {
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
                        Sender = sender == null ? new UserDTO { Id = m.SenderId } : new UserDTO
                        {
                            Id = sender.Id,
                            Username = sender.Username,
                            IsOnline = sender.IsOnline,
                            ProfileImageUrl = sender.ProfileImageUrl,
                            CreatedAt = sender.CreatedAt
                        }
                    };
                })
                .ToList();
        }
    }
}