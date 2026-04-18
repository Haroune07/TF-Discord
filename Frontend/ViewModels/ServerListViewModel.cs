using Frontend.Services;
using Frontend.Global;
using Frontend.ViewModels.Base;
using System.Collections.ObjectModel;

namespace Frontend.ViewModels
{
    public class ServerListViewModel : BaseViewModel
    {
        public ObservableCollection<ServerViewModel> Servers { get; set; }
        private readonly Action<string> _onServerSelected;
        private readonly ApiService _apiService = new();
        public event Action<string>? OnServerSelected;
        public ServerListViewModel(Action<string> onServerSelected)
        {
            _onServerSelected = onServerSelected;
            Servers = new ObservableCollection<ServerViewModel>();
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
                Servers.Add(new ServerViewModel(s.Name, s.Id, (id) =>
                {
                    OnServerSelected?.Invoke(id);
                    _onServerSelected(id);
                }));
            }
        }
    }
}