using Frontend.Commands;
using Frontend.Global;
using Frontend.Services;
using Frontend.ViewModels.Base;
using Frontend.Views;
using Shared.DTOs;
using System.Net.Http;
using System.Windows.Input;
using System.Net.Http.Json;


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
            set { 
                _isDMMode = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(IsServerMode));
                System.Diagnostics.Debug.WriteLine($"IsDMMode={_isDMMode}, IsServerMode={!_isDMMode}");
            }
        }
        public bool IsServerMode => !IsDMMode;
        public string MemberSince => User != null
            ? $"Member since {User.CreatedAt:MMMM dd, yyyy}"
            : "Member since unknown";

        public ICommand? GoToLoginCommand { get; }
        public ICommand? GoToHomeCommand { get; }



        public HomeViewModel(MainViewModel main)
        {
            _main = main;
            GoToLoginCommand = new RelayCommand(Logout, () => true);
            GoToHomeCommand = new RelayCommand(SwitchToDMMode, () => true);
            

            var apiService = new ApiService();
            var chatService = new ChatService();
            ActiveChat = new ChatViewModel(apiService, chatService);

            CurrentUserAvatar = new(User);
            Profile = new ProfileViewModel(apiService, Logout);

            UserList = new UserListViewModel(async (channelId) =>
            {
                IsDMMode = true;
                await ActiveChat.LoadChannelAsync(channelId);
            });

            _ = chatService.ConnectAsync();

            if (_main?.ChannelList != null)
                _main.ChannelList.OnChannelSelected += async (id) => await SelectChannelAsync(id);

            if (_main?.ServerList != null)
            {
                _main.ServerList.OnServerSelected += (id) => IsDMMode = false;
                _ = _main.ServerList.LoadServersAsync();
            }
            CurrentUserAvatar.Refresh();
        }

        










        private void SwitchToDMMode()
        {
            IsDMMode = true;
            _ = UserList.LoadUsersAsync();
        }

        private void Logout()
        {
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