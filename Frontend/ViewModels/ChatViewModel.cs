using Frontend.Commands;
using Frontend.Global;
using Frontend.Services;
using Frontend.ViewModels.Base;
using Shared.DTOs;
using Shared.DTOs.Requests;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Frontend.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private readonly ChatService _chatService;
        private string? _currentChannelId;

        public ObservableCollection<MessageDTO> Messages { get; } = new();

        private string _inputText = string.Empty;
        public string InputText
        {
            get => _inputText;
            set { _inputText = value; OnPropertyChanged(); _ = HandleTypingAsync(); }
        }

        private string _typingText = string.Empty;
        public string TypingText
        {
            get => _typingText;
            set { _typingText = value; OnPropertyChanged(); }
        }

        // Edit state
        private MessageDTO? _editingMessage;
        public MessageDTO? EditingMessage
        {
            get => _editingMessage;
            set { _editingMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsEditing)); }
        }
        public bool IsEditing => _editingMessage != null;

        private CancellationTokenSource? _typingCTS;

        public ICommand SendMessageCommand { get; }
        public ICommand EditMessageCommand { get; }
        public ICommand DeleteMessageCommand { get; }
        public ICommand CancelEditCommand { get; }

        public ChatViewModel(ApiService apiService, ChatService chatService)
        {
            _apiService = apiService;
            _chatService = chatService;

            SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);
            EditMessageCommand = new RelayCommand<MessageDTO>(StartEdit, msg => msg?.Sender?.Id == Session.Current.User?.Id);
            DeleteMessageCommand = new RelayCommand<MessageDTO>(async msg => await DeleteMessageAsync(msg!), msg => msg?.Sender?.Id == Session.Current.User?.Id);
            CancelEditCommand = new RelayCommand(CancelEdit, () => true);

            _chatService.MessageReceived += OnMessageReceived;
            _chatService.UserTyping += OnOtherUserTyping;
            _chatService.UserStoppedTyping += OnOtherUserStoppedTyping;
        }

        public async Task LoadChannelAsync(string channelId)
        {
            if (_currentChannelId == channelId) return;

            if (!string.IsNullOrEmpty(_currentChannelId))
                await _chatService.LeaveChannelAsync(_currentChannelId);

            _currentChannelId = channelId;
            Messages.Clear();
            CancelEdit();

            var history = await _apiService.GetMessagesAsync(channelId);
            foreach (var msg in history)
                Messages.Add(msg);

            await _chatService.JoinChannelAsync(channelId);
        }

        private void StartEdit(MessageDTO? msg)
        {
            if (msg == null) return;
            EditingMessage = msg;
            InputText = msg.Content;
        }

        private void CancelEdit()
        {
            EditingMessage = null;
            InputText = string.Empty;
        }

        private async void SendMessage()
        {
            string content = InputText;

            if (IsEditing)
            {
                await ConfirmEditAsync(content);
                return;
            }

            InputText = string.Empty;
            if (string.IsNullOrEmpty(_currentChannelId)) return;

            var req = new CreateMessageRequest
            {
                ChannelId = _currentChannelId,
                Content = content,
                SenderId = Session.Current.User!.Id
            };
            var savedMessage = await _apiService.SendMessageAsync(req);

            if (savedMessage != null)
            {
                Application.Current.Dispatcher.Invoke(() => Messages.Add(savedMessage));
                await _chatService.BroadcastMessageAsync(savedMessage);
            }
        }

        private async Task ConfirmEditAsync(string newContent)
        {
            if (EditingMessage == null) return;

            var updated = await _apiService.EditMessageAsync(EditingMessage.Id, new EditMessageRequest
            {
                RequesterId = Session.Current.User!.Id,
                NewContent = newContent
            });

            if (updated != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var index = Messages.IndexOf(EditingMessage);
                    if (index >= 0)
                        Messages[index] = updated;
                });
            }

            CancelEdit();
        }

        private async Task DeleteMessageAsync(MessageDTO msg)
        {
            var success = await _apiService.DeleteMessageAsync(msg.Id, Session.Current.User!.Id);

            if (success)
            {
                Application.Current.Dispatcher.Invoke(() => Messages.Remove(msg));
            }
        }

        private bool CanSendMessage() => !string.IsNullOrWhiteSpace(InputText) && !string.IsNullOrEmpty(_currentChannelId);

        private void OnMessageReceived(MessageDTO msg)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (msg.ChannelId == _currentChannelId)
                    Messages.Add(msg);
            });
        }

        private void OnOtherUserTyping(string username)
        {
            Application.Current.Dispatcher.Invoke(() => TypingText = $"{username} is typing...");
        }

        private void OnOtherUserStoppedTyping(string username)
        {
            Application.Current.Dispatcher.Invoke(() => TypingText = string.Empty);
        }

        private async Task HandleTypingAsync()
        {
            if (string.IsNullOrEmpty(_currentChannelId)) return;

            _typingCTS?.Cancel();
            _typingCTS = new CancellationTokenSource();

            await _chatService.NotifyTypingStartedAsync(_currentChannelId, Session.Current.User!.Username);

            try
            {
                await Task.Delay(2000, _typingCTS.Token);
                await _chatService.NotifyTypingStoppedAsync(_currentChannelId, Session.Current.User!.Username);
            }
            catch (TaskCanceledException) { }
        }
    }
}
