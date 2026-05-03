using Frontend.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Frontend.ViewModels
{
    public partial class ChannelListViewModel : ObservableObject
    {
        public ObservableCollection<ChannelViewModel> Channels { get; set; }
        private readonly ApiService _apiService = new();

        public event Action<string>? OnChannelSelected;

        public ChannelListViewModel()
        {
            Channels = new ObservableCollection<ChannelViewModel>();
        }

        public void Clear()
        {
            Channels.Clear();
            OnChannelSelected = null;
        }

        public async Task LoadChannelsAsync(string serverId)
        {
            Channels.Clear();
            var data = await _apiService.GetServerChannelsAsync(serverId);
            foreach (var c in data.Where(c => c.ServerId == serverId))
            {
                Channels.Add(new ChannelViewModel((id) => OnChannelSelected?.Invoke(id))
                {
                    Name = c.Name,
                    Id = c.Id,
                    ServerID = c.ServerId,
                    CreatedAt = c.CreatedAt
                });
            }
        }
    }
}