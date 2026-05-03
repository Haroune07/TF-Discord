using CommunityToolkit.Mvvm.Input;


public class ChannelViewModel
{
    public string Id { get; set; } = string.Empty;
    public string ServerID { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public IRelayCommand SelectCommand { get; }

    public ChannelViewModel(Action<string> onSelected)
    {
        SelectCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => onSelected(Id), () => true);
    }

}
