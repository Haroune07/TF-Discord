using Frontend.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.SignalR;
using System.Windows;


namespace Frontend.ViewModels
{
    public partial class ChannelListViewModel : ObservableObject
    {
        public ObservableCollection<ChannelViewModel> Channels { get; set; }
        private readonly ApiService _apiService;
        private readonly ChatService _chatService;


        public event Action<string>? OnChannelSelected;

        // Dans ChannelListViewModel.cs

        public ChannelListViewModel(ApiService apiService, ChatService chatService)
        {
            _apiService = apiService;
            _chatService = chatService;

            Channels = new ObservableCollection<ChannelViewModel>();

            _chatService.MessageReceived += (message) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var channel = Channels.FirstOrDefault(c => c.Id == message.ChannelId);
                    // On incrémente seulement si ce n'est pas le canal actif (logique optionnelle)
                    if (channel != null)
                    {
                        // On utilise la propriété Visible pour stocker le compte
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