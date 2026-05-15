using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


public partial class ChannelViewModel : ObservableObject
{
    public string Id { get; set; } = string.Empty;
    public string ServerID { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    [ObservableProperty]
    public int visible = 0;

    public DateTime CreatedAt { get; set; }

    public IRelayCommand SelectCommand { get; }

    public ChannelViewModel(Action<string> onSelected)
    {
        SelectCommand = new RelayCommand(() => onSelected(Id), () => true);
    }

}
