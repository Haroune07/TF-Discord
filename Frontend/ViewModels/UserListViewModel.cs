using Frontend.Commands;
using Frontend.Global;
using Frontend.Services;
using Frontend.ViewModels.Base;
using Shared.DTOs;
using Shared.DTOs.Requests;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;


namespace Frontend.ViewModels
{
    public class UserListViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new();
        private readonly Action<string> _onDMChannelReady;

        public ICommand OpenDMCommand { get; }

        public ObservableCollection<UserDTO> Users { get; set; } = new();

        public UserListViewModel(Action<string> onDMChannelReady)
        {
            _onDMChannelReady = onDMChannelReady;
            OpenDMCommand = new RelayCommand<UserDTO>(async (user) => await OpenDMAsync(user!), (user) => user != null);

        }

        public async Task LoadUsersAsync()
        {
            if (Session.Current.User == null) return;

            Users.Clear();
            var users = await _apiService.GetAllUsersExceptMeAsync(Session.Current.User.Id);
            foreach (var u in users)
                Users.Add(u);
        }

        public async Task OpenDMAsync(UserDTO targetUser)
        {
            if (Session.Current.User == null) return;

            var channel = await _apiService.CreateDMAsync(new CreateDMRequest
            {
                SenderId = Session.Current.User.Id,
                TargetUserId = targetUser.Id
            });

            if (channel != null)
                _onDMChannelReady(channel.Id);
        }
    }
}