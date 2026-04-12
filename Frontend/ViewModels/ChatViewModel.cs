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

        private CancellationTokenSource? _typingCTS;

        public ICommand SendMessageCommand { get; }

        public ChatViewModel(ApiService apiService, ChatService chatService)
        {
            _apiService = apiService;
            _chatService = chatService;

            SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);

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

            var history = await _apiService.GetMessagesAsync(channelId);
            foreach (var msg in history)
            {
                Messages.Add(msg);
            }

            //rejoindre le groupe SignalR pour le temps réel
            await _chatService.JoinChannelAsync(channelId);
        }

        private async void SendMessage()
        {
            string content = InputText;
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
                // On broadcast le message sauvegardé (avec le vrai sender du backend)
                await _chatService.BroadcastMessageAsync(savedMessage);
            }
        }

        private bool CanSendMessage() => !string.IsNullOrWhiteSpace(InputText) && !string.IsNullOrEmpty(_currentChannelId);

        private void OnMessageReceived(MessageDTO msg)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (msg.ChannelId == _currentChannelId)
                {
                    Messages.Add(msg);
                }
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