using System;
using System.Threading.Tasks;
using Shared.DTOs;

namespace Frontend.Services
{
    public interface IChatService
    {
        event Action<string>? UserTyping;
        event Action<string>? UserStoppedTyping;
        event Action<MessageDTO>? MessageReceived;
        event Action<MessageDTO>? MessageEdited;
        event Action<string>? MessageDeleted;
        event Action<bool>? ReconnectingChanged;
        event Action<string, string>? KickedFromServer;

        Task ConnectAsync();
        Task JoinChannelAsync(string channelId);
        Task LeaveChannelAsync(string channelId);
        Task BroadcastMessageAsync(MessageDTO messageDTO);
        Task BroadcastMessageEditedAsync(MessageDTO messageDTO);
        Task BroadcastMessageDeletedAsync(string channelId, string messageId);
        Task DisconnectAsync();
        Task NotifyTypingStartedAsync(string channelId, string username);
        Task NotifyTypingStoppedAsync(string channelId, string username);
    }
}
