using Frontend.ViewModels;
using Frontend.ViewModels.Base;

public class MainViewModel : BaseViewModel
{
    private BaseViewModel _currentViewModel;

    public ServerListViewModel ServerList { get; }
    public ChannelListViewModel ChannelList { get; }

    public BaseViewModel CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel = value;
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }

    public MainViewModel()
    {
        ChannelList = new ChannelListViewModel();
        ServerList = new ServerListViewModel(serverId => ChannelList.LoadChannels(serverId));
        _currentViewModel = new LoginViewModel(this);
    }
}