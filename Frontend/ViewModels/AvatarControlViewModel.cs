using Frontend.Global;
using Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;


namespace Frontend.ViewModels
{
    public partial class AvatarControlViewModel : ObservableObject
    {
        private UserDTO? _sourceUser;

        [ObservableProperty]
        private string initials = string.Empty;

        [ObservableProperty]
        private double avatarOpacity;

        [ObservableProperty]
        private string? onlineStatusImage;

        [ObservableProperty]
        private bool isOnline;

        [ObservableProperty]
        private string onlineStatusText = string.Empty;

        [ObservableProperty]
        private string? avatarImage;


        public AvatarControlViewModel(UserDTO? user = null)
        {
            _sourceUser = user ?? Session.Current.User;
            UpdateFromDto();
        }

        private void UpdateFromDto()
        {
            if (_sourceUser != null)
            {
                string name = _sourceUser.Username ?? "??";
                Initials = name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name.ToUpper();
                AvatarImage = _sourceUser.ProfileImageUrl;
                IsOnline = _sourceUser.IsOnline;
                AvatarOpacity = string.IsNullOrEmpty(AvatarImage) ? 0 : 1;
                
                // Using pack URI for absolute safety in WPF resources
                OnlineStatusImage = IsOnline
                    ? "pack://application:,,,/Static/Images/online.png"
                    : "pack://application:,,,/Static/Images/invisible.png";
                
                OnlineStatusText = IsOnline ? "En ligne" : "Invisible";
            }
        }

        public void Refresh()
        {
            UpdateFromDto();
        }
    }
}
