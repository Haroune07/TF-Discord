using Backend.Src.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shared.DTOs;
using Shared.DTOs.Auth;
using Shared.DTOs.Requests;

namespace Backend.Hubs
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

    }
}
