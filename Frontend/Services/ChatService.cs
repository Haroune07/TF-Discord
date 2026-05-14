using Microsoft.AspNetCore.SignalR.Client;
using Shared.Constants;
using Shared.DTOs;

namespace Frontend.Services
{
    public class ChatService : IChatService
    {
        public event Action<string>? UserTyping;
        public event Action<string>? UserStoppedTyping;
        private HubConnection? _connection;
        public event Action<MessageDTO>? MessageReceived;
        public event Action<MessageDTO>? MessageEdited;
        public event Action<string>? MessageDeleted;

        public async Task ConnectAsync()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(Ports.SERVER_LISTEN_URL + "/hubs/chat")
                .WithAutomaticReconnect()
                .Build();

            _connection.On<MessageDTO>("ReceiveMessage", msg => MessageReceived?.Invoke(msg));
            _connection.On<MessageDTO>("MessageEdited", msg => MessageEdited?.Invoke(msg));
            _connection.On<string>("MessageDeleted", msgId => MessageDeleted?.Invoke(msgId));

            _connection.On<string>("UserTyping", username => UserTyping?.Invoke(username));
            _connection.On<string>("UserStoppedTyping", username => UserStoppedTyping?.Invoke(username));

            await _connection.StartAsync();
        }

        public async Task JoinChannelAsync(string channelId) => await _connection!.InvokeAsync("JoinChannel", channelId);
        public async Task LeaveChannelAsync(string channelId) => await _connection!.InvokeAsync("LeaveChannel", channelId);
        
        public async Task BroadcastMessageAsync(MessageDTO messageDTO) => await _connection!.InvokeAsync("SendMessage", messageDTO);
        public async Task BroadcastMessageEditedAsync(MessageDTO messageDTO) => await _connection!.InvokeAsync("EditMessage", messageDTO);
        public async Task BroadcastMessageDeletedAsync(string channelId, string messageId) => await _connection!.InvokeAsync("DeleteMessage", channelId, messageId);

        public async Task DisconnectAsync()
        {
            if (_connection is not null) await _connection.StopAsync();
        }

        public async Task NotifyTypingStartedAsync(string channelId, string username) => await _connection!.InvokeAsync("TypingStarted", channelId, username);
        public async Task NotifyTypingStoppedAsync(string channelId, string username) => await _connection!.InvokeAsync("TypingStopped", channelId, username);
    }
}
