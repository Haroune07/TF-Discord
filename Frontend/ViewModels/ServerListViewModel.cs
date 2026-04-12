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

        public ServerListViewModel(Action<string> onServerSelected)
        {
            _onServerSelected = onServerSelected;
            Servers = new ObservableCollection<ServerViewModel>();
        }

        public async Task LoadServersAsync()
        {
            if (Session.Current.User == null) return;

            Servers.Clear(); // On vide la liste pour éviter les doublons

            var data = await _apiService.GetMyServersAsync(Session.Current.User.Id);

            foreach (var s in data)
            {
                // On utilise le callback sauvegardé pour que le clic fonctionne toujours
                Servers.Add(new ServerViewModel(s.Name, s.Id, _onServerSelected));
            }
        }
    }
}