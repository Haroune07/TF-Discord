using Microsoft.AspNetCore.SignalR;
using Shared.DTOs;

namespace Backend.Src.Hubs
{
    
    public class ChatHub : Hub
    {

        public async Task JoinChannel(string channelId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channelId);
        }

        public async Task LeaveChannel(string channelId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId);
        }

        public async Task SendMessage(MessageDTO messageDTO)
        {
            await Clients.Group(messageDTO.ChannelId).SendAsync("ReceiveMessage", messageDTO);
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
