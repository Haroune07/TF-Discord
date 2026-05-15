using Frontend.Global;
using Frontend.Services;
using Frontend.Views;
using Shared.DTOs.Requests;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public partial class ServerListViewModel : ObservableObject
    {
        public ObservableCollection<ServerViewModel> Servers { get; set; }

        private readonly Func<string, Task> _onServerSelected;
        private readonly IApiService _apiService;

        private ServerViewModel? _selectedServer;
        private int _selectionVersion;

        public event Action<string>? OnServerSelected;
        public event Action? OnServerLeft;

        public string? SelectedServerId => _selectedServer?.Id;

        public IRelayCommand CreateServerCommand { get; }
        public IRelayCommand<ServerViewModel> LeaveServerCommand { get; }
        public IRelayCommand<ServerViewModel> CopyServerIdCommand { get; }
        public IRelayCommand<ServerViewModel> InviteToServerCommand { get; }

        public ServerListViewModel(IApiService apiService, Func<string, Task> onServerSelected)
        {
            _apiService = apiService;
            _onServerSelected = onServerSelected;

            Servers = new ObservableCollection<ServerViewModel>();

            CreateServerCommand = new RelayCommand(OpenCreateServerWindow);
            LeaveServerCommand = new RelayCommand<ServerViewModel>(LeaveServer);
            CopyServerIdCommand = new RelayCommand<ServerViewModel>(CopyServerId);
            InviteToServerCommand = new RelayCommand<ServerViewModel>(InviteToServer);
        }

        public void Clear()
        {
            Servers.Clear();
            _selectedServer = null;
            OnServerSelected = null;
        }

        public async Task LoadServersAsync()
        {
            if (Session.Current.User == null)
                return;

            var previousSelectedId = _selectedServer?.Id;
            Servers.Clear();
            _selectedServer = null;

            var data = await _apiService.GetMyServersAsync(Session.Current.User.Id);

            foreach (var s in data.DistinctBy(s => s.Id))
            {
                Servers.Add(new ServerViewModel(
                    s.Name,
                    s.Id,
                    SelectServerAsync,
                    s.ServerImageUrl
                ));
            }

            if (Servers.Count == 0)
            {
                OnServerLeft?.Invoke();
                return;
            }

            var toSelect = Servers.FirstOrDefault(s => s.Id == previousSelectedId) ?? Servers[0];
            await SelectServerAsync(toSelect.Id);
        }

        public async Task HandleKickedFromServerAsync(string serverId)
        {
            var server = Servers.FirstOrDefault(s => s.Id == serverId);
            if (server == null)
                return;

            var wasSelected = _selectedServer == server;
            Servers.Remove(server);

            MessageBox.Show(
                "Vous avez été expulsé de ce serveur.",
                "Serveur",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            if (!wasSelected)
                return;

            _selectedServer = null;

            if (Servers.Count > 0)
                await SelectServerAsync(Servers[0].Id);
            else
                OnServerLeft?.Invoke();
        }

        public async Task SelectServerAsync(string id)
        {
            var clicked = Servers.FirstOrDefault(s => s.Id == id);
            if (clicked == null)
                return;

            var version = Interlocked.Increment(ref _selectionVersion);

            if (_selectedServer != null)
                _selectedServer.IsSelected = false;

            clicked.IsSelected = true;
            _selectedServer = clicked;

            OnServerSelected?.Invoke(id);
            await _onServerSelected(id);

            if (version != _selectionVersion)
                return;
        }

        public async Task InviteUserToServerAsync(string serverId, string username)
        {
            if (Session.Current.User == null || string.IsNullOrWhiteSpace(username))
                return;

            var users = await _apiService.SearchUsersAsync(username);
            var target = users.FirstOrDefault(u =>
                u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase));

            if (target == null)
            {
                MessageBox.Show($"Utilisateur « {username} » introuvable.", "Invitation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (target.Id == Session.Current.User.Id)
            {
                MessageBox.Show("Vous êtes déjà sur ce serveur.", "Invitation",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var existingMembers = await _apiService.GetServerMembersAsync(serverId);
            if (existingMembers.Any(m => m.User.Id == target.Id))
            {
                MessageBox.Show($"{target.Username} est déjà membre de ce serveur.",
                    "Invitation", MessageBoxButton.OK, MessageBoxImage.Information);
                if (_selectedServer?.Id == serverId)
                    OnServerSelected?.Invoke(serverId);
                return;
            }

            var joined = await _apiService.JoinServerAsync(new JoinOrLeaveServerRequest
            {
                ServerId = serverId,
                UserId = target.Id
            });

            if (!joined)
            {
                MessageBox.Show(
                    "L'invitation a échoué. Réessayez.",
                    "Invitation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"{target.Username} a été ajouté au serveur.", "Invitation",
                MessageBoxButton.OK, MessageBoxImage.Information);

            if (_selectedServer?.Id == serverId)
                OnServerSelected?.Invoke(serverId);
        }

        private void InviteToServer(ServerViewModel? server)
        {
            if (server == null)
                return;

            var window = new InviteToServerWindow
            {
                Owner = Application.Current.MainWindow
            };

            if (window.ShowDialog() != true)
                return;

            _ = InviteUserToServerAsync(server.Id, window.Username);
        }

        private void OpenCreateServerWindow()
        {
            var window = new CreateServerWindow
            {
                Owner = Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                _ = LoadServersAsync();
        }

        private async void LeaveServer(ServerViewModel? server)
        {
            if (server == null || Session.Current.User == null)
                return;

            var confirm = MessageBox.Show(
                $"Quitter le serveur {server.Name} ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            var left = await _apiService.LeaveServerAsync(new JoinOrLeaveServerRequest
            {
                ServerId = server.Id,
                UserId = Session.Current.User.Id
            });

            if (!left)
            {
                MessageBox.Show("Impossible de quitter le serveur.", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var wasSelected = _selectedServer == server;
            Servers.Remove(server);

            if (wasSelected)
            {
                _selectedServer = null;

                if (Servers.Count > 0)
                    await SelectServerAsync(Servers[0].Id);
                else
                    OnServerLeft?.Invoke();
            }
        }

        private static void CopyServerId(ServerViewModel? server)
        {
            if (server == null)
                return;

            Clipboard.SetText(server.Id);
            MessageBox.Show("ID du serveur copié.", "Serveur",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
