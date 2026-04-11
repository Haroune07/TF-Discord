
using Frontend.Models;
using Frontend.ViewModels.Base;
using System.Collections.ObjectModel;


namespace Frontend.ViewModels
{
    public class ChannelListViewModel : BaseViewModel
    {
        public ObservableCollection<ChannelViewModel> Channels { get; set; }

        public ChannelListViewModel()
        {
            Channels = new ObservableCollection<ChannelViewModel>();
        }

        public void LoadChannels(string serverId)
        {
            Channels.Clear();
            var data = MockServerData.GetFakeChannels();
            foreach (var c in data.Where(c => c.ServerId == serverId))
            {
                Channels.Add(new ChannelViewModel { Name = c.Name, Id = c.Id, ServerID = c.ServerId, CreatedAt = c.CreatedAt });
            }
        }
    }
}