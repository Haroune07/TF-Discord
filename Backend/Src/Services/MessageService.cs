using Backend.Src.Models;
using Backend.Src.Repository;
using Shared.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Src.Services
{
    public class MessageService
    {
        private readonly IRepository<Message> _messages;

        public MessageService(IRepository<Message> messageRepo)
        {
            _messages = messageRepo;
        }

        public async Task<Message> SendMessageAsync(CreateMessageRequest req)
        {
            var message = new Message
            {
                Content = req.Content,
                ChannelId = req.ChannelId,
                SenderId = req.SenderId,
                SentAt = DateTime.Now
            };

            await _messages.InsertAsync(message);
            return message;
        }

        public async Task<List<Message>> GetMessagesByChannelAsync(string channelId)
        {
            var messages = await _messages.FindAsync(m => m.ChannelId == channelId);
            return messages.OrderBy(m => m.SentAt).ToList();
        }
    }
}
