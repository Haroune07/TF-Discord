using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public partial class UserInviteViewModel : ObservableObject
    {
        [ObservableProperty]
        private string username = string.Empty;

        public string Id { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ProfileImageUrl { get; set; }
        public IRelayCommand DMCommand { get; }
        public IRelayCommand InviteServerCommand { get; }

        public UserInviteViewModel(Action<string> onDM, Action<string> onInvite)
        {
            DMCommand = new RelayCommand(() => onDM(Id), () => !string.IsNullOrEmpty(Id));
            InviteServerCommand = new RelayCommand(() => onInvite(Id), () => !string.IsNullOrEmpty(Id));
        }
    }
}
