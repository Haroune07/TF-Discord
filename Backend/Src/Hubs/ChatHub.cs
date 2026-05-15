using Backend.Src.Services;
using Microsoft.AspNetCore.SignalR;
using Shared.DTOs;

namespace Backend.Src.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChannelService _channelService;

        public ChatHub(ChannelService channelService)
        {
            _channelService = channelService;
        }

        public async Task JoinChannel(string channelId, string userId)
        {
            if (!await _channelService.CanAccessChannelAsync(channelId, userId))
                return;

            await Groups.AddToGroupAsync(Context.ConnectionId, channelId);
        }

        public async Task LeaveChannel(string channelId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId);
        }

        public async Task SendMessage(MessageDTO messageDTO)
        {
            await Clients.OthersInGroup(messageDTO.ChannelId).SendAsync("ReceiveMessage", messageDTO);
        }

        public async Task EditMessage(MessageDTO messageDTO)
        {
            await Clients.OthersInGroup(messageDTO.ChannelId).SendAsync("MessageEdited", messageDTO);
        }

        public async Task DeleteMessage(string channelId, string messageId)
        {
            await Clients.OthersInGroup(channelId).SendAsync("MessageDeleted", messageId);
        }

        public async Task TypingStarted(string channelId, string username)
        {
            await Clients.OthersInGroup(channelId).SendAsync("UserTyping", username);
        }

        public async Task TypingStopped(string channelId, string username)
        {
            await Clients.OthersInGroup(channelId).SendAsync("UserStoppedTyping", username);
        }
    }
}
