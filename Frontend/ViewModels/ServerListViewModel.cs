using Frontend.Models;
using Frontend.ViewModels.Base;
using System.Collections.ObjectModel;
using Frontend.ViewModels;

namespace Frontend.ViewModels
{
    public class ServerListViewModel : BaseViewModel
    {
        public ObservableCollection<ServerViewModel> Servers { get; set; }

        public ServerListViewModel(Action<string> onServerSelected)
        {
            Servers = new ObservableCollection<ServerViewModel>();
            var data = MockServerData.GetFakeServers();
            foreach (var s in data)
            {
                Servers.Add(new ServerViewModel(s.Name, s.Id, onServerSelected));
            }
        }
    }
}