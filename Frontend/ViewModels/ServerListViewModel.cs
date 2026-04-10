using Frontend.Models;
using Frontend.ViewModels.Base;
using System.Collections.ObjectModel;
using Frontend.ViewModels;

public class ServerListViewModel : BaseViewModel
{
    public ObservableCollection<ServerViewModel> Servers { get; set; }

    public ServerListViewModel()
    {
        var fakeData = MockServerData.GetFakeServers();
        Servers = new ObservableCollection<ServerViewModel>(fakeData);
    }
}
