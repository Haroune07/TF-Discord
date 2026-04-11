using Frontend.Models;
using Frontend.ViewModels.Base;
using System.Collections.ObjectModel;

public class ChannelListViewModel : BaseViewModel
{
    public ObservableCollection<ChannelViewModel> Channels { get; set; }

    public ChannelListViewModel() // Constructeur sans paramètre possible ici
    {
        Channels = new ObservableCollection<ChannelViewModel>();
        ServerSelectionService.OnServerChanged += LoadChannels;
    }

    private void LoadChannels(int serverId)
    {
        Channels.Clear();
        var data = MockServerData.GetFakeChannels();

        foreach (var c in data)
        {
            if (c.ServerId == serverId)
            {
                Channels.Add(new ChannelViewModel { Name = c.Name });
            }
        }
    }
}
