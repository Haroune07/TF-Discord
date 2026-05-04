using Frontend.Global;
using Frontend.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;

namespace Frontend.ViewModels
{
    public partial class ServerListViewModel : ObservableObject
    {
        public ObservableCollection<ServerViewModel> Servers { get; set; }
        private readonly Action<string> _onServerSelected;
        private readonly ApiService _apiService = new();
        private ServerViewModel? _selectedServer;
        public event Action<string>? OnServerSelected;
        public IRelayCommand CreateServerCommand { get; }

        public ServerListViewModel(Action<string> onServerSelected)
        {
            _onServerSelected = onServerSelected;
            Servers = new ObservableCollection<ServerViewModel>();
            CreateServerCommand = new RelayCommand(OpenCreateServerWindow);
        }

        public void Clear()
        {
            Servers.Clear();
            OnServerSelected = null;
        }

        public async Task LoadServersAsync()
        {
            if (Session.Current.User == null) return;

            Servers.Clear();

            var data = await _apiService.GetAllServersAsync();

            foreach (var s in data)
            {
                Servers.Add(new ServerViewModel(
                    s.Name,
                    s.Id,
                    (id) =>
                    {
                        var clicked = Servers.First(s => s.Id == id);
                        if (clicked == _selectedServer)
                            return;

                        if (_selectedServer != null)
                            _selectedServer.IsSelected = false;

                        clicked.IsSelected = true;
                        _selectedServer = clicked;



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