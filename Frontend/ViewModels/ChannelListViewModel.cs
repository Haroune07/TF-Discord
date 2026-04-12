
using Frontend.Models;
using Frontend.Services;
using Frontend.ViewModels.Base;
using System.Collections.ObjectModel;


namespace Frontend.ViewModels
{
    public class ChannelListViewModel : BaseViewModel
    {
        public ObservableCollection<ChannelViewModel> Channels { get; set; }
        private readonly ApiService _apiService = new();

        public event Action<string>? OnChannelSelected;

        public ChannelListViewModel()
        {
            Channels = new ObservableCollection<ChannelViewModel>();
        }

        public async Task LoadChannelsAsync(string serverId)
        {
            Channels.Clear();
            var data = await _apiService.GetServerChannelsAsync(serverId);
            foreach (var c in data.Where(c => c.ServerId == serverId))
            {
                Channels.Add(new ChannelViewModel(  (id) => OnChannelSelected?.Invoke(id))
                { 
                    Name = c.Name, 
                    Id = c.Id, 
                    ServerID = c.ServerId, 
                    CreatedAt = 
                    c.CreatedAt
                });
            }
        }
    }
}