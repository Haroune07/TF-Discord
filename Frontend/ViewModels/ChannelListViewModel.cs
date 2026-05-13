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
        private readonly IApiService _apiService;
        private readonly IChatService _chatService;
        private readonly IDispatcherService _dispatcher;

        public event Action<string>? OnChannelSelected;

        public ChannelListViewModel(IApiService apiService, IChatService chatService, IDispatcherService dispatcher)
        {
            _apiService = apiService;
            _chatService = chatService;
            _dispatcher = dispatcher;

            Channels = new ObservableCollection<ChannelViewModel>();

            _chatService.MessageReceived += (message) =>
            {
                _dispatcher.Invoke(() =>
                {
                    var channel = Channels.FirstOrDefault(c => c.Id == message.ChannelId);
                    if (channel != null)
                    {
                        channel.Visible = 1;
                    }
                });
            };
        }

        public async Task LoadChannelsAsync(string serverId)
        {
            Channels.Clear();
            var data = await _apiService.GetServerChannelsAsync(serverId);
            foreach (var c in data.Where(c => c.ServerId == serverId))
            {
                Channels.Add(new ChannelViewModel((id) => HandleSelect(id))
                {
                    Name = c.Name,
                    Id = c.Id,
                    ServerID = c.ServerId,
                    CreatedAt = c.CreatedAt,
                });
            }
        }

        private void HandleSelect(string channelId)
        {
            var channel = Channels.FirstOrDefault(c => c.Id == channelId);
            if (channel != null)
            {
                channel.Visible = 0;
            }

            OnChannelSelected?.Invoke(channelId);
        }


        public void Clear()
        {
            Channels.Clear();
            OnChannelSelected = null;
        }

    }
}