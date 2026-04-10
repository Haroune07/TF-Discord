using Frontend.Models;
using Frontend.ViewModels.Base;
using System.Collections.ObjectModel;

public class ServerListViewModel : BaseViewModel
{
    public ObservableCollection<Server> Servers { get; set; }

    public ServerListViewModel()
    {
        var fakeData = MockServerData.GetFakeServers();
        Servers = new ObservableCollection<Server>(fakeData);
    }
}
