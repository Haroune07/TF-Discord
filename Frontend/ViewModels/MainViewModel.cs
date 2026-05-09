using Frontend.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject currentViewModel;

    public ServerListViewModel ServerList { get; }
    public ChannelListViewModel ChannelList { get; }

    public MainViewModel(ChannelListViewModel channelList, ServerListViewModel serverList, IServiceProvider services)
    {
        ChannelList = channelList;
        ServerList = serverList;
        currentViewModel = ActivatorUtilities.CreateInstance<LoginViewModel>(services, this);
    }

    public void ResetState()
    {
        ServerList.Clear();
        ChannelList.Clear();
        Frontend.Global.Session.Current.Logout();
    }
}
