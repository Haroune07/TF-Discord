using Frontend.Global;
using Microsoft.AspNetCore.SignalR.Client;
//using Microsoft.AspNetCore.SignalR.Client;
using Shared.Constants;
using Shared.DTOs;
using System.Runtime.Intrinsics.Arm;


namespace Frontend.Services
{
    public class ChatService
    {

        private HubConnection? _connection;

        public event Action<MessageDTO>? MessageReceived;

        public async Task ConnectAsync()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(Ports.SERVER_LISTEN_URL + "/hubs/chat")
                .WithAutomaticReconnect()
                .Build();

            _connection.On<MessageDTO>("ReceiveMessage", msg =>
            {
                MessageReceived?.Invoke(msg);
            });

            await _connection.StartAsync();

        }

        public async Task JoinChannelAsync(string channelId)
        {
            await _connection!.InvokeAsync("JoinChannel", channelId);
        }

        public async Task LeaveChannelAsync(string channelId)
            => await _connection!.InvokeAsync("LeaveChannel", channelId);

        public async Task SendMessageAsync(string channelId, string content)
        {
            var messageDTO = new MessageDTO
            {
                ChannelId = channelId,
                Content = content,
                Sender = Session.Current.User!,
                SentAt = DateTime.UtcNow
            };

            await _connection!.InvokeAsync("SendMessage", messageDTO);

        }

        public async Task DisconnectAsync()
        {
            if (_connection is not null)
                await _connection.StopAsync();
        }

    }
}
