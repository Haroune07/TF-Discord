using Backend.Src.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shared.DTOs;
using Shared.DTOs.Auth;
using Shared.DTOs.Requests;

namespace Backend.Hubs
{
    
    public class MainHub : Hub
    {
        public async Task SendMessage(MessageDTO messageDTO) {

            await Clients.All.SendAsync("SendMessage", messageDTO);
        }

    }
}
