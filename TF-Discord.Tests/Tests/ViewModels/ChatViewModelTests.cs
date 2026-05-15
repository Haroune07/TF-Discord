using Frontend.Global;
using Frontend.Services;
using Frontend.ViewModels;
using Moq;
using Shared.DTOs;
using Shared.DTOs.Requests;

namespace TF_Discord.Tests.Tests.ViewModels;

/// <summary>
/// Unit tests for ChatViewModel (DTF-74).
/// Uses mocked IApiService, IChatService, and IDispatcherService to verify
/// messaging reliability without any WPF or network dependency.
/// </summary>
public class ChatViewModelTests
{
    // ─── Helpers ────────────────────────────────────────────────────────────

    private static readonly UserDTO TestUser = new()
    {
        Id = "user-1",
        Username = "Alice"
    };

    private static readonly string TestChannelId = "channel-abc";

    /// <summary>Seed the global Session with a test user before each scenario.</summary>
    private static void SetupSession()
    {
        Session.Current.Login(TestUser);
    }

    /// <summary>
    /// Build a fully-mocked ChatViewModel. The dispatcher mock simply invokes the
    /// action synchronously so that assertions can run immediately.
    /// </summary>
    private static (ChatViewModel vm, Mock<IApiService> api, Mock<IChatService> chat) Build()
    {
        SetupSession();

        var apiMock = new Mock<IApiService>();
        var chatMock = new Mock<IChatService>();
        var dispatcherMock = new Mock<IDispatcherService>();

        // Make Invoke() call the action immediately (synchronous in tests)
        dispatcherMock
            .Setup(d => d.Invoke(It.IsAny<Action>()))
            .Callback<Action>(a => a());

        dispatcherMock
            .Setup(d => d.InvokeAsync(It.IsAny<Action>()))
            .Callback<Action>(a => a())
            .Returns(Task.CompletedTask);

        // Wire up IChatService events so Moq can raise them
        chatMock.SetupAdd(c => c.MessageReceived += It.IsAny<Action<MessageDTO>>());
        chatMock.SetupAdd(c => c.MessageEdited += It.IsAny<Action<MessageDTO>>());
        chatMock.SetupAdd(c => c.MessageDeleted += It.IsAny<Action<string>>());
        chatMock.SetupAdd(c => c.UserTyping += It.IsAny<Action<string>>());
        chatMock.SetupAdd(c => c.UserStoppedTyping += It.IsAny<Action<string>>());
        chatMock.SetupAdd(c => c.ReconnectingChanged += It.IsAny<Action<bool>>());

        chatMock.Setup(c => c.JoinChannelAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        chatMock.Setup(c => c.LeaveChannelAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        chatMock.Setup(c => c.BroadcastMessageAsync(It.IsAny<MessageDTO>())).Returns(Task.CompletedTask);
        chatMock.Setup(c => c.BroadcastMessageEditedAsync(It.IsAny<MessageDTO>())).Returns(Task.CompletedTask);
        chatMock.Setup(c => c.BroadcastMessageDeletedAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        chatMock.Setup(c => c.NotifyTypingStartedAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        chatMock.Setup(c => c.NotifyTypingStoppedAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        var vm = new ChatViewModel(apiMock.Object, chatMock.Object, dispatcherMock.Object);
        return (vm, apiMock, chatMock);
    }

    // ─── LoadChannelAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task LoadChannelAsync_PopulatesMessages_WhenHistoryExists()
    {
        var (vm, api, _) = Build();

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id))
           .ReturnsAsync(new List<MessageDTO>
           {
               new() { Id = "m1", Content = "Hello", ChannelId = TestChannelId, Sender = TestUser },
               new() { Id = "m2", Content = "World", ChannelId = TestChannelId, Sender = TestUser }
           });

        await vm.LoadChannelAsync(TestChannelId);

        Assert.Equal(2, vm.Messages.Count);
        Assert.Equal("Hello", vm.Messages[0].Message.Content);
        Assert.Equal("World", vm.Messages[1].Message.Content);
    }

    [Fact]
    public async Task LoadChannelAsync_ClearsOldMessages_WhenSwitchingChannel()
    {
        var (vm, api, _) = Build();
        var secondChannel = "channel-xyz";

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id))
           .ReturnsAsync(new List<MessageDTO>
           {
               new() { Id = "m1", Content = "Old", ChannelId = TestChannelId, Sender = TestUser }
           });

        api.Setup(a => a.GetMessagesAsync(secondChannel, TestUser.Id))
           .ReturnsAsync(new List<MessageDTO>
           {
               new() { Id = "m2", Content = "New", ChannelId = secondChannel, Sender = TestUser }
           });

        await vm.LoadChannelAsync(TestChannelId);
        await vm.LoadChannelAsync(secondChannel);

        Assert.Single(vm.Messages);
        Assert.Equal("New", vm.Messages[0].Message.Content);
    }

    [Fact]
    public async Task LoadChannelAsync_DoesNotReload_WhenSameChannelRequested()
    {
        var (vm, api, _) = Build();

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO>());

        await vm.LoadChannelAsync(TestChannelId);
        await vm.LoadChannelAsync(TestChannelId); // second call should be a no-op

        api.Verify(a => a.GetMessagesAsync(TestChannelId, TestUser.Id), Times.Once);
    }

    [Fact]
    public async Task LoadChannelAsync_JoinsChannel_ViaSignalR()
    {
        var (vm, api, chat) = Build();

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO>());

        await vm.LoadChannelAsync(TestChannelId);

        chat.Verify(c => c.JoinChannelAsync(TestChannelId), Times.Once);
    }

    [Fact]
    public async Task LoadChannelAsync_LeavesOldChannel_WhenSwitching()
    {
        var (vm, api, chat) = Build();
        var secondChannel = "channel-xyz";

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO>());
        api.Setup(a => a.GetMessagesAsync(secondChannel, TestUser.Id)).ReturnsAsync(new List<MessageDTO>());

        await vm.LoadChannelAsync(TestChannelId);
        await vm.LoadChannelAsync(secondChannel);

        chat.Verify(c => c.LeaveChannelAsync(TestChannelId), Times.Once);
    }

    // ─── SendMessage ────────────────────────────────────────────────────────

    [Fact]
    public async Task SendMessage_AddsMessageToCollection_OnSuccess()
    {
        var (vm, api, _) = Build();

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO>());
        await vm.LoadChannelAsync(TestChannelId);

        var saved = new MessageDTO { Id = "new-1", Content = "Hi", ChannelId = TestChannelId, Sender = TestUser };
        api.Setup(a => a.SendMessageAsync(It.IsAny<CreateMessageRequest>())).ReturnsAsync(saved);

        vm.InputText = "Hi";
        vm.SendMessageCommand.Execute(null);

        // Small delay to let async void SendMessage complete
        await Task.Delay(100);

        Assert.Single(vm.Messages);
        Assert.Equal("Hi", vm.Messages[0].Message.Content);
    }

    [Fact]
    public async Task SendMessage_BroadcastsViaSignalR_OnSuccess()
    {
        var (vm, api, chat) = Build();

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO>());
        await vm.LoadChannelAsync(TestChannelId);

        var saved = new MessageDTO { Id = "new-1", Content = "Hi", ChannelId = TestChannelId, Sender = TestUser };
        api.Setup(a => a.SendMessageAsync(It.IsAny<CreateMessageRequest>())).ReturnsAsync(saved);

        vm.InputText = "Hi";
        vm.SendMessageCommand.Execute(null);

        await Task.Delay(100);

        chat.Verify(c => c.BroadcastMessageAsync(saved), Times.Once);
    }

    [Fact]
    public async Task SendMessage_DoesNotAdd_WhenApiReturnsNull()
    {
        var (vm, api, _) = Build();

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO>());
        await vm.LoadChannelAsync(TestChannelId);

        api.Setup(a => a.SendMessageAsync(It.IsAny<CreateMessageRequest>()))
           .ReturnsAsync((MessageDTO?)null);

        vm.InputText = "Hi";
        vm.SendMessageCommand.Execute(null);

        await Task.Delay(100);

        Assert.Empty(vm.Messages);
    }

    [Fact]
    public void SendMessageCommand_CannotExecute_WhenInputIsEmpty()
    {
        var (vm, _, _) = Build();

        vm.InputText = string.Empty;

        Assert.False(vm.SendMessageCommand.CanExecute(null));
    }

    [Fact]
    public async Task SendMessageCommand_CannotExecute_WithoutChannel()
    {
        var (vm, api, _) = Build();

        // No LoadChannelAsync — channel is null
        vm.InputText = "Something";

        await Task.Delay(50);

        Assert.False(vm.SendMessageCommand.CanExecute(null));
    }

    // ─── Edit flow ──────────────────────────────────────────────────────────

    [Fact]
    public async Task EditMessageCommand_SetsEditingState_AndPopulatesInputText()
    {
        var (vm, api, _) = Build();

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id))
           .ReturnsAsync(new List<MessageDTO>
           {
               new() { Id = "m1", Content = "Original", ChannelId = TestChannelId, Sender = TestUser }
           });

        await vm.LoadChannelAsync(TestChannelId);

        var item = vm.Messages[0];
        vm.EditMessageCommand.Execute(item);

        Assert.True(vm.IsEditing);
        Assert.Equal(item, vm.EditingMessage);
        Assert.Equal("Original", vm.InputText);
    }

    [Fact]
    public async Task CancelEditCommand_ClearsEditingState()
    {
        var (vm, api, _) = Build();

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id))
           .ReturnsAsync(new List<MessageDTO>
           {
               new() { Id = "m1", Content = "Original", ChannelId = TestChannelId, Sender = TestUser }
           });

        await vm.LoadChannelAsync(TestChannelId);
        vm.EditMessageCommand.Execute(vm.Messages[0]);

        vm.CancelEditCommand.Execute(null);

        Assert.False(vm.IsEditing);
        Assert.Null(vm.EditingMessage);
        Assert.Equal(string.Empty, vm.InputText);
    }

    [Fact]
    public async Task SendMessage_WhenEditing_CallsEditApi_AndUpdatesMessage()
    {
        var (vm, api, chat) = Build();

        var original = new MessageDTO { Id = "m1", Content = "Original", ChannelId = TestChannelId, Sender = TestUser };
        var updated = new MessageDTO { Id = "m1", Content = "Edited", ChannelId = TestChannelId, Sender = TestUser, IsEdited = true };

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO> { original });
        api.Setup(a => a.EditMessageAsync("m1", It.IsAny<EditMessageRequest>())).ReturnsAsync(updated);

        await vm.LoadChannelAsync(TestChannelId);
        vm.EditMessageCommand.Execute(vm.Messages[0]);

        vm.InputText = "Edited";
        vm.SendMessageCommand.Execute(null);

        await Task.Delay(100);

        Assert.Equal("Edited", vm.Messages[0].Message.Content);
        Assert.True(vm.Messages[0].Message.IsEdited);
        Assert.False(vm.IsEditing);
        chat.Verify(c => c.BroadcastMessageEditedAsync(updated), Times.Once);
    }

    // ─── Delete flow ────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteMessageCommand_RemovesMessage_OnSuccess()
    {
        var (vm, api, _) = Build();

        var msg = new MessageDTO { Id = "m1", Content = "Bye", ChannelId = TestChannelId, Sender = TestUser };
        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO> { msg });
        api.Setup(a => a.DeleteMessageAsync("m1", TestUser.Id)).ReturnsAsync(true);

        await vm.LoadChannelAsync(TestChannelId);
        var item = vm.Messages[0];

        vm.DeleteMessageCommand.Execute(item);
        await Task.Delay(100);

        Assert.Empty(vm.Messages);
    }

    [Fact]
    public async Task DeleteMessageCommand_BroadcastsDeletion_OnSuccess()
    {
        var (vm, api, chat) = Build();

        var msg = new MessageDTO { Id = "m1", Content = "Bye", ChannelId = TestChannelId, Sender = TestUser };
        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO> { msg });
        api.Setup(a => a.DeleteMessageAsync("m1", TestUser.Id)).ReturnsAsync(true);

        await vm.LoadChannelAsync(TestChannelId);
        vm.DeleteMessageCommand.Execute(vm.Messages[0]);

        await Task.Delay(100);

        chat.Verify(c => c.BroadcastMessageDeletedAsync(TestChannelId, "m1"), Times.Once);
    }

    [Fact]
    public async Task DeleteMessageCommand_DoesNotRemove_WhenApiFails()
    {
        var (vm, api, _) = Build();

        var msg = new MessageDTO { Id = "m1", Content = "Keep", ChannelId = TestChannelId, Sender = TestUser };
        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO> { msg });
        api.Setup(a => a.DeleteMessageAsync("m1", TestUser.Id)).ReturnsAsync(false);

        await vm.LoadChannelAsync(TestChannelId);
        vm.DeleteMessageCommand.Execute(vm.Messages[0]);

        await Task.Delay(100);

        Assert.Single(vm.Messages);
    }

    // ─── SignalR event handling ─────────────────────────────────────────────

    [Fact]
    public async Task OnMessageReceived_AddsMessage_WhenChannelMatches()
    {
        var (vm, api, chat) = Build();

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO>());
        await vm.LoadChannelAsync(TestChannelId);

        // Simulate SignalR push
        var incomingMsg = new MessageDTO { Id = "sig-1", Content = "Incoming", ChannelId = TestChannelId, Sender = TestUser };
        chat.Raise(c => c.MessageReceived += null, incomingMsg);

        Assert.Single(vm.Messages);
        Assert.Equal("Incoming", vm.Messages[0].Message.Content);
    }

    [Fact]
    public async Task OnMessageReceived_IgnoresMessage_WhenChannelDoesNotMatch()
    {
        var (vm, api, chat) = Build();

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO>());
        await vm.LoadChannelAsync(TestChannelId);

        var wrongChannel = new MessageDTO { Id = "sig-x", Content = "Wrong", ChannelId = "other-channel", Sender = TestUser };
        chat.Raise(c => c.MessageReceived += null, wrongChannel);

        Assert.Empty(vm.Messages);
    }

    [Fact]
    public async Task OnMessageEdited_UpdatesExistingMessage_WhenChannelMatches()
    {
        var (vm, api, chat) = Build();

        var original = new MessageDTO { Id = "m1", Content = "Old", ChannelId = TestChannelId, Sender = TestUser };
        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO> { original });
        await vm.LoadChannelAsync(TestChannelId);

        var edited = new MessageDTO { Id = "m1", Content = "Updated", ChannelId = TestChannelId, Sender = TestUser, IsEdited = true };
        chat.Raise(c => c.MessageEdited += null, edited);

        Assert.Equal("Updated", vm.Messages[0].Message.Content);
    }

    [Fact]
    public async Task OnMessageDeleted_RemovesMessage_ByMessageId()
    {
        var (vm, api, chat) = Build();

        var msg = new MessageDTO { Id = "m1", Content = "ToDelete", ChannelId = TestChannelId, Sender = TestUser };
        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO> { msg });
        await vm.LoadChannelAsync(TestChannelId);

        chat.Raise(c => c.MessageDeleted += null, "m1");

        Assert.Empty(vm.Messages);
    }

    // ─── Typing notifications ───────────────────────────────────────────────

    [Fact]
    public async Task OnUserTyping_SetsTypingText()
    {
        var (vm, api, chat) = Build();

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO>());
        await vm.LoadChannelAsync(TestChannelId);

        chat.Raise(c => c.UserTyping += null, "Bob");

        Assert.Contains("Bob", vm.TypingText);
    }

    [Fact]
    public async Task OnUserStoppedTyping_ClearsTypingText()
    {
        var (vm, api, chat) = Build();

        api.Setup(a => a.GetMessagesAsync(TestChannelId, TestUser.Id)).ReturnsAsync(new List<MessageDTO>());
        await vm.LoadChannelAsync(TestChannelId);

        chat.Raise(c => c.UserTyping += null, "Bob");
        chat.Raise(c => c.UserStoppedTyping += null, "Bob");

        Assert.Equal(string.Empty, vm.TypingText);
    }
}
