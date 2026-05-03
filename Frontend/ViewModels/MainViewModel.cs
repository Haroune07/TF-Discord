using Frontend.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject currentViewModel;

    public ServerListViewModel ServerList { get; }
    public ChannelListViewModel ChannelList { get; }

    public MainViewModel()
    {
        ChannelList = new ChannelListViewModel();
        ServerList = new ServerListViewModel(async serverId => await ChannelList.LoadChannelsAsync(serverId));
        currentViewModel = new LoginViewModel(this);
    }

    public void ResetState()
    {
        ServerList.Clear();
        ChannelList.Clear();
        Frontend.Global.Session.Current.Logout();
    }
}