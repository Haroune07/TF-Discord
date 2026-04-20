
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shared.DTOs
{
    public class UserDTO : INotifyPropertyChanged
    {
        private string _profileImageUrl = string.Empty;
        private string _onlineStatus = "Offline";
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string OnlineStatus
        {
            get => _onlineStatus;
            set
            {
                _onlineStatus = value;
                OnPropertyChanged();
            }
        }
        public DateTime CreatedAt { get; set; }
        public string ProfileImageUrl
        {
            get => _profileImageUrl;
            set
            {
                _profileImageUrl = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}