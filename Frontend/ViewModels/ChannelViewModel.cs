using Frontend.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

public class ChannelViewModel
{
    public string Id { get; set; } = string.Empty;
    public string ServerID { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public ICommand SelectCommand { get; }

    public ChannelViewModel(Action<string> onSelected)
    {
        SelectCommand = new RelayCommand(() => onSelected(Id), () => true);
    }

}
