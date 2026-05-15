using Frontend.Global;
using Frontend.Services;
using Shared.DTOs.Requests;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Frontend.ViewModels
{
    public partial class SearchUserViewModel : ObservableObject
    {
        public ObservableCollection<UserInviteViewModel> Users { get; } = new();
        private readonly IApiService _apiService;

        public event Action<string>? OnDMRequest;
        public Func<string, string, Task>? OnInviteToServer { get; set; }

        public bool HasResults => Users.Count > 0;

        [ObservableProperty]
        private string inputText = string.Empty;

        [ObservableProperty]
        private string selectedServerId = string.Empty;

        [ObservableProperty]
        private bool canInviteToServer;

        partial void OnInputTextChanged(string value) => _ = LoadUserInvite(value);

        partial void OnSelectedServerIdChanged(string value)
        {
            CanInviteToServer = !string.IsNullOrEmpty(value);
            foreach (var user in Users)
                user.RefreshInviteState(CanInviteToServer);
        }

        public SearchUserViewModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task LoadUserInvite(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                Users.Clear();
                OnPropertyChanged(nameof(HasResults));
                return;
            }

            try
            {
                var data = await _apiService.SearchUsersAsync(query);
                Users.Clear();

                foreach (var c in data)
                {
                    var inviteVm = new UserInviteViewModel(
                        onDM: async id => await HandleOpenDM(id),
                        onInvite: async id => await HandleServerInvite(id, c.Username),
                        canInvite: () => CanInviteToServer)
                    {
                        Id = c.Id,
                        Username = c.Username,
                        ProfileImageUrl = c.ProfileImageUrl
                    };
                    inviteVm.RefreshInviteState(CanInviteToServer);
                    Users.Add(inviteVm);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur LoadUserInvite: {ex.Message}");
            }
            finally
            {
                OnPropertyChanged(nameof(HasResults));
            }
        }

        private async Task HandleOpenDM(string userId)
        {
            if (Session.Current.User == null) return;

            var channel = await _apiService.CreateDMAsync(new CreateDMRequest
            {
                SenderId = Session.Current.User.Id,
                TargetUserId = userId
            });

            if (channel != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    InputText = string.Empty;
                    OnDMRequest?.Invoke(channel.Id);
                });
            }
        }

        private async Task HandleServerInvite(string userId, string username)
        {
            if (string.IsNullOrEmpty(SelectedServerId))
            {
                System.Windows.MessageBox.Show(
                    "Sélectionnez d'abord un serveur dans la barre latérale.",
                    "Invitation",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            if (OnInviteToServer != null)
            {
                await OnInviteToServer(SelectedServerId, username);
                InputText = string.Empty;
                return;
            }

            if (Session.Current.User == null) return;

            var joined = await _apiService.JoinServerAsync(new JoinOrLeaveServerRequest
            {
                ServerId = SelectedServerId,
                UserId = userId
            });

            InputText = string.Empty;

            if (joined)
            {
                System.Windows.MessageBox.Show(
                    $"{username} a été ajouté au serveur.",
                    "Invitation",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show(
                    $"{username} est peut-être déjà membre de ce serveur.",
                    "Invitation",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
            }
        }
    }
}
