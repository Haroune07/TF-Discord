using Frontend.Commands;
using Frontend.Global;
using Frontend.Services;
using Frontend.ViewModels.Base;
using Shared.DTOs;
using System.Windows.Input;

namespace Frontend.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private MainViewModel? _main;

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
                System.Diagnostics.Debug.WriteLine($"IsDMMode={_isDMMode}, IsServerMode={!_isDMMode}");
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

        private readonly ChatService _chatService; // déjà présent, s'assurer qu'il est au niveau de la classe


        public HomeViewModel(MainViewModel main)
        {
            _main = main;
            GoToLoginCommand = new RelayCommand(Logout, () => true);
            GoToHomeCommand = new RelayCommand(SwitchToDMMode, () => true);

            var apiService = new ApiService();
            _chatService = new ChatService();
            ActiveChat = new ChatViewModel(apiService, _chatService);

            CurrentUserAvatar = new(User);

            UserList = new UserListViewModel(async (channelId) =>
            {
                IsDMMode = true;
                await ActiveChat.LoadChannelAsync(channelId);
            });

            _ = _chatService.ConnectAsync();

            if (_main?.ChannelList != null)
                _main.ChannelList.OnChannelSelected += async (id) => await SelectChannelAsync(id);

            if (_main?.ServerList != null)
            {
                _main.ServerList.OnServerSelected += (id) => IsDMMode = false;
                _ = _main.ServerList.LoadServersAsync();
            }
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