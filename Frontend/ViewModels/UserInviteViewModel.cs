using Frontend.Commands;
using Frontend.ViewModels.Base;
using System;
using System.Windows.Input;

namespace Frontend.ViewModels
{
    public class UserInviteViewModel : BaseViewModel
    {
        private string _username = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Username 
        { 
            get => _username; 
            set { if (_username != value) { _username = value; OnPropertyChanged(); } } 
        }
        public bool IsOnline { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ProfileImageUrl { get; set; }
        public ICommand DMCommand { get; }
        public ICommand InviteServerCommand { get; }

        public UserInviteViewModel(Action<string> onDM, Action<string> onInvite)
        {
            DMCommand = new RelayCommand(() => onDM(Id), () => !string.IsNullOrEmpty(Id));
            InviteServerCommand = new RelayCommand(() => onInvite(Id), () => !string.IsNullOrEmpty(Id));
        }
    }
}
