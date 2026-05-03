using Frontend.Global;
using Frontend.Services;
using Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Threading.Tasks;

namespace Frontend.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private MainViewModel? _main;
        public ProfileViewModel Profile { get; }

        public UserDTO? User { get; private set; } = Session.Current.User;
        public AvatarControlViewModel CurrentUserAvatar { get; set; }

        public ServerListViewModel? ServerList => _main?.ServerList;
        public ChannelListViewModel? ChannelList => _main?.ChannelList;
        public UserListViewModel UserList { get; }

        public ChatViewModel ActiveChat { get; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsServerMode))]
        private bool _isDMMode = false;

        partial void OnIsDMModeChanged(bool value)
        {
            if (value) _ = UserList.LoadUsersAsync();
        }

        public bool IsServerMode => !IsDMMode;

        public bool IsUserOnline => User?.IsOnline == true;
        public string OnlineStatus => IsUserOnline ? "Online" : "Offline";
        public string MemberSince => User != null
            ? $"Member since {User.CreatedAt:MMMM dd, yyyy}"
            : "Member since unknown";

        public IRelayCommand? GoToLoginCommand { get; }
        public IRelayCommand? GoToHomeCommand { get; }

        private readonly ChatService _chatService;
        private readonly ApiService _apiService;
        public SearchUserViewModel SearchVM { get; set; } = new SearchUserViewModel();

        public HomeViewModel(MainViewModel main)
        {
            _main = main;
            GoToLoginCommand = new RelayCommand(Logout);
            GoToHomeCommand = new RelayCommand(SwitchToDMMode);

            _apiService = new ApiService();
            _chatService = new ChatService();
            ActiveChat = new ChatViewModel(_apiService, _chatService);

            CurrentUserAvatar = new(User);
            Profile = new ProfileViewModel(_apiService, Logout);

            UserList = new UserListViewModel(async (channelId) =>
            {
                IsDMMode = true;
                await ActiveChat.LoadChannelAsync(channelId);
            });

            SearchVM.OnDMRequest += (channelId) =>
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    IsDMMode = true;
                    await UserList.LoadUsersAsync();
                    await ActiveChat.LoadChannelAsync(channelId);
                });
            };

            _ = _chatService.ConnectAsync();

            // Use named handlers to allow proper unsubscription
            if (_main?.ChannelList != null)
                _main.ChannelList.OnChannelSelected += OnChannelSelected;

            if (_main?.ServerList != null)
            {
                _main.ServerList.OnServerSelected += OnServerSelected;
                _ = _main.ServerList.LoadServersAsync();
            }
            CurrentUserAvatar.Refresh();

            // Load initial data
            _ = UserList.LoadUsersAsync();
        }

        private void SwitchToDMMode()
        {
            IsDMMode = true;
        }

        private async void Logout()
        {
            // 1. Unsubscribe from global events to prevent ghost calls
            if (_main?.ChannelList != null)
                _main.ChannelList.OnChannelSelected -= OnChannelSelected;

            if (_main?.ServerList != null)
                _main.ServerList.OnServerSelected -= OnServerSelected;

            // 2. Clear state and disconnect
            await _chatService.DisconnectAsync();
            _main?.ResetState();

            // 3. Navigate back to login
            _main!.CurrentViewModel = new LoginViewModel(_main);
        }

        private async void OnChannelSelected(string id)
        {
            await SelectChannelAsync(id);
        }

        private void OnServerSelected(string id)
        {
            IsDMMode = false;
        }

        public async Task SelectChannelAsync(string channelId)
        {
            IsDMMode = false;
            await ActiveChat.LoadChannelAsync(channelId);
        }
    }
}
