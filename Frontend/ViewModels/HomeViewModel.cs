using Frontend.Commands;
using Frontend.Global;
using Frontend.Services;
using Frontend.ViewModels.Base;
using Shared.DTOs;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Frontend.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private MainViewModel? _main;

        // Données utilisateur
        public UserDTO? User { get; private set; } = Session.Current.User;
        public AvatarControlViewModel CurrentUserAvatar { get; set; }

        // Accès aux listes de Wilson via MainViewModel
        public ServerListViewModel? ServerList => _main?.ServerList;
        public ChannelListViewModel? ChannelList => _main?.ChannelList;

        // Ton Chat
        public ChatViewModel ActiveChat { get; }

        // Propriétés de statut
        public bool IsUserOnline => User?.IsOnline == true;
        public string OnlineStatus => IsUserOnline ? "Online" : "Offline";
        public string MemberSince => User != null
            ? $"Member since {User.CreatedAt:MMMM dd, yyyy}"
            : "Member since unknown";

        public ICommand? GoToLoginCommand { get; }

        public HomeViewModel(MainViewModel main)
        {
            _main = main;
            GoToLoginCommand = new RelayCommand(Logout, () => true);

            var apiService = new ApiService();
            var chatService = new ChatService();
            ActiveChat = new ChatViewModel(apiService, chatService);

            CurrentUserAvatar = new(User);

            // demarrer la connexion SignalR en arrière-plan
            _ = chatService.ConnectAsync();

            // s'abonner au clic sur un salon
            if (_main?.ChannelList != null)
            {
                _main.ChannelList.OnChannelSelected += async (id) => await SelectChannelAsync(id);
            }

            // Charger les serveurs maintenant que le login est fait
            if (_main?.ServerList != null)
            {
                _ = _main.ServerList.LoadServersAsync();
            }
        }

        private void Logout()
        {
            Session.Current.Logout();
            _main!.CurrentViewModel = new LoginViewModel(_main);
        }

        public async Task SelectChannelAsync(string channelId)
        {
            await ActiveChat.LoadChannelAsync(channelId);
        }
    }
}