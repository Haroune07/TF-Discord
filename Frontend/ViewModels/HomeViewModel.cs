using Frontend.Commands;
using Frontend.Global;
using Frontend.Services;
using Frontend.ViewModels.Base;
using Shared.DTOs;
using System.Windows;
using System.Windows.Input;

namespace Frontend.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private MainViewModel? _main;
        public ProfileViewModel Profile { get; }

        public UserDTO? User { get; private set; } = Session.Current.User;
        public AvatarControlViewModel CurrentUserAvatar { get; set; }

        public ServerListViewModel? ServerList => _main?.ServerList;
        public ChannelListViewModel? ChannelList => _main?.ChannelList;
        public UserListViewModel UserList { get; }

        public ChatViewModel ActiveChat { get; }

        private bool _isDMMode = false;
        public bool IsDMMode
        {
            get => _isDMMode;
            set
            {
                _isDMMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsServerMode));
                if (_isDMMode) _ = UserList.LoadUsersAsync();
            }
        }
        public bool IsServerMode => !IsDMMode;

        public bool IsUserOnline => User?.IsOnline == true;
        public string OnlineStatus => IsUserOnline ? "Online" : "Offline";
        public string MemberSince => User != null
            ? $"Member since {User.CreatedAt:MMMM dd, yyyy}"
            : "Member since unknown";

        public ICommand? GoToLoginCommand { get; }
        public ICommand? GoToHomeCommand { get; }

        private readonly ChatService _chatService;
        private readonly ApiService _apiService;
        public SearchUserViewModel SearchVM { get; set; } = new SearchUserViewModel();

        public HomeViewModel(MainViewModel main)
        {
            _main = main;
            GoToLoginCommand = new RelayCommand(Logout, () => true);
            GoToHomeCommand = new RelayCommand(SwitchToDMMode, () => true);

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

            if (_main?.ChannelList != null)
                _main.ChannelList.OnChannelSelected += async (id) => await SelectChannelAsync(id);

            if (_main?.ServerList != null)
            {
                _main.ServerList.OnServerSelected += (id) => IsDMMode = false;
                _ = _main.ServerList.LoadServersAsync();
            }
            CurrentUserAvatar.Refresh();

            // Load initial data
            _ = UserList.LoadUsersAsync();
        }

        private void SwitchToDMMode()
        {
            IsDMMode = true;
            _ = UserList.LoadUsersAsync();
        }

        private async void Logout()
        {
            await _chatService.DisconnectAsync();
            Session.Current.Logout();
            _main!.CurrentViewModel = new LoginViewModel(_main);
        }

        public async Task SelectChannelAsync(string channelId)
        {
            IsDMMode = false;
            await ActiveChat.LoadChannelAsync(channelId);
        }

    }
}
