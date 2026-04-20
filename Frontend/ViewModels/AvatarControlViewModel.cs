using Frontend.Global;
using Frontend.ViewModels.Base;
using Shared.DTOs; // <--- VÉRIFIE QUE CETTE LIGNE EST BIEN LÀ

namespace Frontend.ViewModels
{
    public class AvatarControlViewModel : BaseViewModel
    {
        private UserDTO? _sourceUser;

        public string Initials { get; set; } = string.Empty;
        public double AvatarOpacity { get; set; }

        
        public string? OnlineStatusImage { get; set; }

        private bool _isOnline;
        private string _onlineStatusText;
        private string? _avatarImage;

        public bool IsOnline
        {
            get => _isOnline;
            set { _isOnline = value; OnPropertyChanged(); }
        }

        public string OnlineStatusText
        {
            get => _onlineStatusText;
            set { _onlineStatusText = value; OnPropertyChanged(); }
        }

        public string? AvatarImage
        {
            get => _avatarImage;
            set { _avatarImage = value; OnPropertyChanged(); }
        }

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
                OnlineStatusImage = IsOnline
                    ? "pack://application:,,,/Static/Images/online.png"
                    : "pack://application:,,,/Static/Images/invisible.png";
            }
        }

        public void Refresh()
        {
            if (_sourceUser != null)
            {
                // En ré-assignant les propriétés, le "set" appelle OnPropertyChanged automatiquement
                IsOnline = _sourceUser.IsOnline;
                OnlineStatusText = IsOnline ? "En ligne" : "Invisible";
                AvatarImage = _sourceUser.ProfileImageUrl;

                // Relance aussi le calcul des initiales si besoin
                string name = _sourceUser.Username ?? "??";
                Initials = name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name.ToUpper();
            }
        }
    }
}
