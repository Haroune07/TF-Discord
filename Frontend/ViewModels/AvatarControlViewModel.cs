using Frontend.Global;
using Frontend.ViewModels.Base;
using Shared.DTOs;

namespace Frontend.ViewModels
{
    public class AvatarControlViewModel : BaseViewModel
    {
        private UserDTO? _sourceUser;

        private string _initials = string.Empty;
        public string Initials
        {
            get => _initials;
            set { _initials = value; OnPropertyChanged(); }
        }

        private double _avatarOpacity;
        public double AvatarOpacity
        {
            get => _avatarOpacity;
            set { _avatarOpacity = value; OnPropertyChanged(); }
        }

        private string? _onlineStatusImage;
        public string? OnlineStatusImage
        {
            get => _onlineStatusImage;
            set { _onlineStatusImage = value; OnPropertyChanged(); }
        }

        private bool _isOnline;
        public bool IsOnline
        {
            get => _isOnline;
            set { _isOnline = value; OnPropertyChanged(); }
        }

        private string _onlineStatusText = string.Empty;
        public string OnlineStatusText
        {
            get => _onlineStatusText;
            set { _onlineStatusText = value; OnPropertyChanged(); }
        }

        private string? _avatarImage;
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
                string name = _sourceUser.Username ?? \"??\";
                Initials = name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name.ToUpper();
                AvatarImage = _sourceUser.ProfileImageUrl;
                IsOnline = _sourceUser.IsOnline;
                AvatarOpacity = string.IsNullOrEmpty(AvatarImage) ? 0 : 1;
                OnlineStatusImage = IsOnline
                    ? \"/Static/Images/online.png\"
                    : \"/Static/Images/invisible.png\";
                OnlineStatusText = IsOnline ? \"En ligne\" : \"Invisible\";
            }
        }

        public void Refresh()
        {
            UpdateFromDto();
        }
    }
}
