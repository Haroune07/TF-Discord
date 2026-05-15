using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public partial class UserInviteViewModel : ObservableObject
    {
        [ObservableProperty]
        private string username = string.Empty;

        public string Id { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }

        [ObservableProperty]
        private bool canInviteToServer;

        public IRelayCommand DMCommand { get; }
        public IRelayCommand InviteServerCommand { get; }

        public UserInviteViewModel(
            Func<string, Task> onDM,
            Func<string, Task> onInvite,
            Func<bool> canInvite)
        {
            DMCommand = new AsyncRelayCommand(async () => await onDM(Id), () => !string.IsNullOrEmpty(Id));
            InviteServerCommand = new AsyncRelayCommand(
                async () => await onInvite(Id),
                () => !string.IsNullOrEmpty(Id) && canInvite());
        }

        public void RefreshInviteState(bool canInvite)
        {
            CanInviteToServer = canInvite;
            InviteServerCommand.NotifyCanExecuteChanged();
        }

        partial void OnCanInviteToServerChanged(bool value) =>
            InviteServerCommand.NotifyCanExecuteChanged();
    }
}
