using Frontend.Models;
using Frontend.ViewModels.Base;
using System.Collections.ObjectModel;
using Frontend.ViewModels;

public class ServerListViewModel : BaseViewModel
{
    public ObservableCollection<ServerViewModel> Servers { get; set; }

    public ServerListViewModel()
    {
        Servers = new ObservableCollection<ServerViewModel>();
        var data = MockServerData.GetFakeServers();
        foreach (var s in data)
        {
            Servers.Add(new ServerViewModel { Name = s.Name, Id = s.Id });
        }
    }

}
