using Frontend.Global;
using Frontend.Services;
using Shared.DTOs;
using Shared.DTOs.Requests;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public partial class MessageItemViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormattedDate))]
        [NotifyPropertyChangedFor(nameof(IsOwnMessage))]
        private MessageDTO message;

        public bool IsOwnMessage => Message.Sender?.Id == Session.Current.User?.Id;

        public string FormattedDate
        {
            get
            {
                var localTime = Message.SentAt.ToLocalTime();
                var now = DateTime.Now;
                if (localTime.Date == now.Date)
                    return $"Aujourd'hui à {localTime:HH:mm}";
                if (localTime.Date == now.AddDays(-1).Date)
                    return $"Hier à {localTime:HH:mm}";
                return $"Le {localTime:dd/MM/yyyy} à {localTime:HH:mm}";
            }
        }

        public MessageItemViewModel(MessageDTO message)
        {
            this.message = message;
        }
    }

    public partial class ChatViewModel : ObservableObject
    {
        private readonly IApiService _apiService;
        private readonly IChatService _chatService;
        private readonly IDispatcherService _dispatcher;
        private string? _currentChannelId;

        public ObservableCollection<MessageItemViewModel> Messages { get; } = new();

        [ObservableProperty]
        private string inputText = string.Empty;

        [ObservableProperty]
        private string typingText = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isReconnecting;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEditing))]
        private MessageItemViewModel? editingMessage;

        public bool IsEditing => EditingMessage != null;

        private CancellationTokenSource? _typingCTS;
        private bool _isSending;

        public IRelayCommand SendMessageCommand { get; }
        public IRelayCommand EditMessageCommand { get; }
        public IRelayCommand DeleteMessageCommand { get; }
        public IRelayCommand CancelEditCommand { get; }

        public ChatViewModel(IApiService apiService, IChatService chatService, IDispatcherService dispatcher)
        {
            _apiService = apiService;
            _chatService = chatService;
            _dispatcher = dispatcher;

            if (apiService is ApiService api)
                api.SetErrorCallback(msg => _dispatcher.Invoke(() => ErrorMessage = msg));

            SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);
            EditMessageCommand = new RelayCommand<MessageItemViewModel>(StartEdit, msg => msg?.IsOwnMessage == true);
            DeleteMessageCommand = new RelayCommand<MessageItemViewModel>(async msg => await DeleteMessageAsync(msg!), msg => msg?.IsOwnMessage == true);
            CancelEditCommand = new RelayCommand(CancelEdit, () => true);

            _chatService.MessageReceived += OnMessageReceived;
            _chatService.MessageEdited += OnMessageEdited;
            _chatService.MessageDeleted += OnMessageDeleted;
            _chatService.UserTyping += OnOtherUserTyping;
            _chatService.UserStoppedTyping += OnOtherUserStoppedTyping;
            _chatService.ReconnectingChanged += OnReconnectingChanged;
        }

        public async Task ClearChannelAsync()
        {
            if (!string.IsNullOrEmpty(_currentChannelId))
                await _chatService.LeaveChannelAsync(_currentChannelId);

            _currentChannelId = null;
            Messages.Clear();
            CancelEdit();
            ErrorMessage = string.Empty;
        }

        public void Detach()
        {
            _chatService.MessageReceived -= OnMessageReceived;
            _chatService.MessageEdited -= OnMessageEdited;
            _chatService.MessageDeleted -= OnMessageDeleted;
            _chatService.UserTyping -= OnOtherUserTyping;
            _chatService.UserStoppedTyping -= OnOtherUserStoppedTyping;
            _chatService.ReconnectingChanged -= OnReconnectingChanged;
        }

        private void OnReconnectingChanged(bool reconnecting)
        {
            _dispatcher.Invoke(() => IsReconnecting = reconnecting);

            if (!reconnecting && !string.IsNullOrEmpty(_currentChannelId))
                _ = _chatService.JoinChannelAsync(_currentChannelId);
        }

        partial void OnInputTextChanged(string value)
        {
            ((RelayCommand)SendMessageCommand).NotifyCanExecuteChanged();
            _ = HandleTypingAsync();
        }

        public async Task LoadChannelAsync(string channelId)
        {
            if (_currentChannelId == channelId) return;

            ErrorMessage = string.Empty;

            if (!string.IsNullOrEmpty(_currentChannelId))
                await _chatService.LeaveChannelAsync(_currentChannelId);

            _currentChannelId = channelId;
            Messages.Clear();
            CancelEdit();

            try
            {
                if (Session.Current.User == null)
                    return;

                var history = await _apiService.GetMessagesAsync(channelId, Session.Current.User.Id);
                foreach (var msg in history)
                    Messages.Add(new MessageItemViewModel(msg));
            }
            catch
            {
                return;
            }

            await _chatService.JoinChannelAsync(channelId);
        }

        private void StartEdit(MessageItemViewModel? msg)
        {
            if (msg == null) return;
            EditingMessage = msg;
            InputText = msg.Message.Content;
        }

        private void CancelEdit()
        {
            EditingMessage = null;
            InputText = string.Empty;
        }

        private async void SendMessage()
        {
            if (_isSending) return;

            ErrorMessage = string.Empty;
            string content = InputText;

            if (IsEditing)
            {
                await ConfirmEditAsync(content);
                return;
            }

            if (string.IsNullOrWhiteSpace(content) || string.IsNullOrEmpty(_currentChannelId))
                return;

            _isSending = true;
            InputText = string.Empty;

            try
            {
                var req = new CreateMessageRequest
                {
                    ChannelId = _currentChannelId,
                    Content = content,
                    SenderId = Session.Current.User!.Id
                };
                var savedMessage = await _apiService.SendMessageAsync(req);

                if (savedMessage != null)
                {
                    _dispatcher.Invoke(() => AddMessageIfNew(savedMessage));
                    await _chatService.BroadcastMessageAsync(savedMessage);
                }
                else
                {
                    _dispatcher.Invoke(() =>
                        ErrorMessage = "Vous n'avez plus accès à ce canal.");
                    InputText = content;
                }
            }
            catch
            {
                InputText = content;
            }
            finally
            {
                _isSending = false;
            }
        }

        private async Task ConfirmEditAsync(string newContent)
        {
            if (EditingMessage == null) return;

            try
            {
                ErrorMessage = string.Empty;
                var updated = await _apiService.EditMessageAsync(EditingMessage.Message.Id, new EditMessageRequest
                {
                    RequesterId = Session.Current.User!.Id,
                    NewContent = newContent
                });

                if (updated != null)
                {
                    _dispatcher.Invoke(() => EditingMessage.Message = updated);
                    await _chatService.BroadcastMessageEditedAsync(updated);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la modification : {ex.Message}");
            }
            finally
            {
                CancelEdit();
            }
        }

        private async Task DeleteMessageAsync(MessageItemViewModel msg)
        {
            try
            {
                ErrorMessage = string.Empty;
                var success = await _apiService.DeleteMessageAsync(msg.Message.Id, Session.Current.User!.Id);

                if (success)
                {
                    _dispatcher.Invoke(() => Messages.Remove(msg));
                    if (_currentChannelId != null)
                        await _chatService.BroadcastMessageDeletedAsync(_currentChannelId, msg.Message.Id);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur inattendue lors de la suppression : {ex.Message}");
            }
        }

        private bool CanSendMessage() => !string.IsNullOrWhiteSpace(InputText) && !string.IsNullOrEmpty(_currentChannelId);

        private void OnMessageReceived(MessageDTO msg)
        {
            _dispatcher.Invoke(() =>
            {
                if (msg.ChannelId == _currentChannelId)
                    AddMessageIfNew(msg);
            });
        }

        private void AddMessageIfNew(MessageDTO msg)
        {
            if (string.IsNullOrEmpty(msg.Id))
            {
                Messages.Add(new MessageItemViewModel(msg));
                return;
            }

            if (Messages.Any(m => m.Message.Id == msg.Id))
                return;

            Messages.Add(new MessageItemViewModel(msg));
        }

        private void OnMessageEdited(MessageDTO msg)
        {
            _dispatcher.Invoke(() =>
            {
                if (msg.ChannelId == _currentChannelId)
                {
                    var existing = Messages.FirstOrDefault(m => m.Message.Id == msg.Id);
                    if (existing != null) existing.Message = msg;
                }
            });
        }

        private void OnMessageDeleted(string msgId)
        {
            _dispatcher.Invoke(() =>
            {
                var existing = Messages.FirstOrDefault(m => m.Message.Id == msgId);
                if (existing != null) Messages.Remove(existing);
            });
        }

        private void OnOtherUserTyping(string username)
        {
            _dispatcher.Invoke(() => TypingText = $"{username} est en train d'écrire...");
        }

        private void OnOtherUserStoppedTyping(string username)
        {
            _dispatcher.Invoke(() => TypingText = string.Empty);
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
