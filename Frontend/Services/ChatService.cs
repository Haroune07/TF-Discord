using Frontend.Global;
using Microsoft.AspNetCore.SignalR.Client;
using Shared.Constants;
using Shared.DTOs;

namespace Frontend.Services
{
    public class ChatService : IChatService
    {
        public event Action<string>? UserTyping;
        public event Action<string>? UserStoppedTyping;
        public event Action<MessageDTO>? MessageReceived;
        public event Action<MessageDTO>? MessageEdited;
        public event Action<string>? MessageDeleted;
        public event Action<bool>? ReconnectingChanged;
        public event Action<string, string>? KickedFromServer;

        private HubConnection? _connection;
        private int _reconnectAttempt;
        private bool _handlersRegistered;

        public async Task ConnectAsync()
        {
            if (_connection?.State == HubConnectionState.Connected)
                return;

            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
                _handlersRegistered = false;
            }

            _connection = new HubConnectionBuilder()
                .WithUrl(Ports.SERVER_LISTEN_URL + "/hubs/chat")
                .WithAutomaticReconnect()
                .Build();

            _connection.Reconnecting += OnReconnectingAsync;
            _connection.Reconnected += OnReconnectedAsync;
            _connection.Closed += OnConnectionClosedAsync;

            if (!_handlersRegistered)
            {
                _connection.On<MessageDTO>("ReceiveMessage", msg => MessageReceived?.Invoke(msg));
                _connection.On<MessageDTO>("MessageEdited", msg => MessageEdited?.Invoke(msg));
                _connection.On<string>("MessageDeleted", msgId => MessageDeleted?.Invoke(msgId));
                _connection.On<string>("UserTyping", username => UserTyping?.Invoke(username));
                _connection.On<string>("UserStoppedTyping", username => UserStoppedTyping?.Invoke(username));
                _connection.On<string, string>("KickedFromServer", (serverId, userId) =>
                    RunOnUi(() => KickedFromServer?.Invoke(serverId, userId)));
                _handlersRegistered = true;
            }

            await _connection.StartAsync();
        }

        private Task OnReconnectingAsync(Exception? _) =>
            RunOnUi(() => ReconnectingChanged?.Invoke(true));

        private Task OnReconnectedAsync(string? _) =>
            RunOnUi(() => ReconnectingChanged?.Invoke(false));

        private static Task RunOnUi(Action action)
        {
            action();
            return Task.CompletedTask;
        }

        private async Task OnConnectionClosedAsync(Exception? _)
        {
            if (_connection is null) return;

            ReconnectingChanged?.Invoke(true);

            while (_connection.State == HubConnectionState.Disconnected)
            {
                var delay = TimeSpan.FromSeconds(Math.Min(Math.Pow(2, _reconnectAttempt++), 30));
                await Task.Delay(delay);

                try
                {
                    await _connection.StartAsync();
                    _reconnectAttempt = 0;
                    ReconnectingChanged?.Invoke(false);
                    return;
                }
                catch
                {
                    // Retry with longer backoff.
                }
            }
        }

        public async Task JoinChannelAsync(string channelId)
        {
            var userId = Session.Current.User?.Id;
            if (string.IsNullOrEmpty(userId))
                return;

            await _connection!.InvokeAsync("JoinChannel", channelId, userId);
        }
        public async Task LeaveChannelAsync(string channelId) => await _connection!.InvokeAsync("LeaveChannel", channelId);

        public async Task BroadcastMessageAsync(MessageDTO messageDTO) => await _connection!.InvokeAsync("SendMessage", messageDTO);
        public async Task BroadcastMessageEditedAsync(MessageDTO messageDTO) => await _connection!.InvokeAsync("EditMessage", messageDTO);
        public async Task BroadcastMessageDeletedAsync(string channelId, string messageId) => await _connection!.InvokeAsync("DeleteMessage", channelId, messageId);

        public async Task DisconnectAsync()
        {
            if (_connection is not null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
                _connection = null;
                _handlersRegistered = false;
            }
        }

        public async Task NotifyTypingStartedAsync(string channelId, string username) => await _connection!.InvokeAsync("TypingStarted", channelId, username);
        public async Task NotifyTypingStoppedAsync(string channelId, string username) => await _connection!.InvokeAsync("TypingStopped", channelId, username);
    }
}
