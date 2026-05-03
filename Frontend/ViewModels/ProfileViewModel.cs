using Frontend.Global;
using Frontend.Services;
using Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Frontend.ViewModels
{
    public class ProfileViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        public UserDTO? User { get; } = Session.Current.User;
        public AvatarControlViewModel Avatar { get; }

        public IAsyncRelayCommand UploadAvatarCommand { get; }
        public IAsyncRelayCommand SetOnlineStatusCommand { get; }
        public IRelayCommand LogoutCommand { get; } // Ajouté pour le bouton Logout

        public ProfileViewModel(ApiService apiService, Action onLogout)
        {
            _apiService = apiService;
            Avatar = new AvatarControlViewModel(User);

            UploadAvatarCommand = new AsyncRelayCommand(async () => await OpenUrlInputDialog(), ()=> true);
            SetOnlineStatusCommand = new AsyncRelayCommand<string>(async (p) => await UpdateStatus(p), parameter => true);
            LogoutCommand = new RelayCommand(onLogout, ()=> true); // Relie l'action de déconnexion

            Avatar.Refresh();
        }

        private async Task UpdateStatus(string parameter)
        {
            if (User == null) return;
            bool success = await _apiService.UpdateStatusAsync(User.Id, parameter);
            if (success)
            {
                User.IsOnline = (parameter == "True");
                Avatar.Refresh();
            }
        }

        private async Task OpenUrlInputDialog()
        {
            var dialog = new Frontend.Views.UrlInputWindowView();
            dialog.Owner = App.Current.MainWindow;
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.Url))
            {
                if (User == null) return;
                bool success = await _apiService.UploadProfileImageAsync(User.Id, dialog.Url);
                if (success)
                {
                    User.ProfileImageUrl = dialog.Url;
                    Avatar.Refresh();
                }
            }
        }
    }
}
