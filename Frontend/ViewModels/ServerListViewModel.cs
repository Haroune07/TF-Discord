using Frontend.Global;
using Frontend.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public class ServerListViewModel : ObservableObject
    {
        public ObservableCollection<ServerViewModel> Servers { get; set; }
        private readonly Action<string> _onServerSelected;
        private readonly ApiService _apiService = new();
        public event Action<string>? OnServerSelected;
        public ICommand CreateServerCommand { get; }

        public ServerListViewModel(Action<string> onServerSelected)
        {
            _onServerSelected = onServerSelected;
            Servers = new ObservableCollection<ServerViewModel>();
            CreateServerCommand = new RelayCommand(OpenCreateServerWindow, () => true);
        }

        public async Task LoadServersAsync()
        {
            if (Session.Current.User == null) return;

            Servers.Clear(); // On vide la liste pour éviter les doublons

            var data = await _apiService.GetAllServers();

            // on devrait utiliser cette méthode, mais en ce moment on teste
            //var data = await _apiService.GetMyServersAsync(Session.Current.User.Id);

            foreach (var s in data)
            {
                Servers.Add(new ServerViewModel(
                    s.Name,
                    s.Id,
                    (id) =>
                    {
                        OnServerSelected?.Invoke(id);
                        _onServerSelected(id);
                    },
                    s.ServerImageUrl
                ));
            }
        }

        private void OpenCreateServerWindow()
        {
            var window = new Frontend.Views.CreateServerWindow();
            window.Owner = Application.Current.MainWindow;
            bool? result = window.ShowDialog();

            if (result == true)
                _ = LoadServersAsync();
        }
    }
}