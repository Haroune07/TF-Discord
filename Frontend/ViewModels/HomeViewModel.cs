using Frontend.Global;
using Frontend.Services;
using Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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

        private readonly IChatService _chatService;
        private readonly IApiService _apiService;
        private readonly IDispatcherService _dispatcher;
        private readonly IServiceProvider _services;
        public SearchUserViewModel SearchVM { get; set; }

        public HomeViewModel(
            MainViewModel main,
            IApiService apiService,
            IChatService chatService,
            IDispatcherService dispatcher,
            ChatViewModel activeChat,
            SearchUserViewModel searchVM,
            IServiceProvider services)
        {
            _main = main;
            _apiService = apiService;
            _chatService = chatService;
            _dispatcher = dispatcher;
            ActiveChat = activeChat;
            SearchVM = searchVM;
            _services = services;

            GoToLoginCommand = new RelayCommand(Logout);
            GoToHomeCommand = new RelayCommand(SwitchToDMMode);

            CurrentUserAvatar = new(User);
            Profile = new ProfileViewModel(_apiService, Logout);

            UserList = new UserListViewModel(_apiService, _dispatcher, async (channelId) =>
            {
                IsDMMode = true;
                await ActiveChat.LoadChannelAsync(channelId);
            });

            SearchVM.OnDMRequest += (channelId) =>
            {
                _dispatcher.Invoke(async () =>
                {
                    IsDMMode = true;
                    await UserList.LoadUsersAsync();
                    await ActiveChat.LoadChannelAsync(channelId);
                });
            };

            _ = _chatService.ConnectAsync();

            if (_main?.ChannelList != null)
                _main.ChannelList.OnChannelSelected += OnChannelSelected;

            if (_main?.ServerList != null)
            {
                _main.ServerList.OnServerSelected += OnServerSelected;
                _ = _main.ServerList.LoadServersAsync();
            }
            CurrentUserAvatar.Refresh();

            _ = UserList.LoadUsersAsync();
        }

        private void SwitchToDMMode()
        {
            IsDMMode = true;
        }

        private async void Logout()
        {
            if (_main?.ChannelList != null)
                _main.ChannelList.OnChannelSelected -= OnChannelSelected;

            if (_main?.ServerList != null)
                _main.ServerList.OnServerSelected -= OnServerSelected;

            await _chatService.DisconnectAsync();
            _main?.ResetState();

            _main!.CurrentViewModel = ActivatorUtilities.CreateInstance<LoginViewModel>(_services, _main);
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
