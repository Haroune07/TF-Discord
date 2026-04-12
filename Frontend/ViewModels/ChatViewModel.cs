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
            set { _inputText = value; OnPropertyChanged(); }
        }

        public ICommand SendMessageCommand { get; }

        public ChatViewModel(ApiService apiService, ChatService chatService)
        {
            _apiService = apiService;
            _chatService = chatService;

            SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);

            _chatService.MessageReceived += OnMessageReceived;
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
            InputText = string.Empty; // Vider l'UI instantanément

            if (string.IsNullOrEmpty(_currentChannelId)) return;

            // 1. Envoi via HTTP pour la base de données
            var req = new CreateMessageRequest { ChannelId = _currentChannelId, Content = content, SenderId = Session.Current.User!.Id };
            var savedMessage = await _apiService.SendMessageAsync(req);

            // 2. Si sauvegardé, on l'envoie aux autres via SignalR
            if (savedMessage != null)
            {
                await _chatService.SendMessageAsync(_currentChannelId, content);
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
    }
}