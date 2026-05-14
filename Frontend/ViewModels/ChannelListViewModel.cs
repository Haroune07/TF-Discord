using Frontend.Global;
using Frontend.Services;
using Frontend.Views;
using Shared.DTOs;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public partial class ChannelListViewModel : ObservableObject
    {
        public ObservableCollection<ChannelViewModel> Channels { get; set; }

        private readonly ApiService _apiService;
        private readonly HttpClient _client = new();

        private string _currentServerId = string.Empty;
        private bool _canManageChannels;

        public bool CanManageChannels
        {
            get => _canManageChannels;
            set => SetProperty(ref _canManageChannels, value);
        }

        public event Action<string>? OnChannelSelected;

        public IRelayCommand CreateChannelCommand { get; }
        public IRelayCommand<ChannelViewModel> DeleteChannelCommand { get; }

        public ChannelListViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Channels = new ObservableCollection<ChannelViewModel>();

            _client.BaseAddress = new Uri(Shared.Constants.Ports.SERVER_LISTEN_URL);

            CreateChannelCommand = new RelayCommand(CreateChannel);
            DeleteChannelCommand = new RelayCommand<ChannelViewModel>(DeleteChannel);
        }

        public void Clear()
        {
            Channels.Clear();
            OnChannelSelected = null;
            _currentServerId = string.Empty;
            CanManageChannels = false;
        }

        public async Task LoadChannelsAsync(string serverId)
        {
            _currentServerId = serverId;

            await LoadCurrentUserRoleAsync(serverId);

            Channels.Clear();
            var data = await _apiService.GetServerChannelsAsync(serverId);

            foreach (var c in data.Where(c => c.ServerId == serverId))
            {
                Channels.Add(new ChannelViewModel((id) => OnChannelSelected?.Invoke(id))
                {
                    Name = c.Name ?? string.Empty,
                    Id = c.Id,
                    ServerID = c.ServerId ?? string.Empty,
                    CreatedAt = c.CreatedAt
                });
            }
        }

        private async Task LoadCurrentUserRoleAsync(string serverId)
        {
            CanManageChannels = false;

            if (Session.Current.User == null)
                return;

            try
            {
                var members = await _client.GetFromJsonAsync<List<ServerMemberDTO>>(
                    $"/api/server/{serverId}/members"
                ) ?? new();

                var currentMember = members.FirstOrDefault(
                    m => m.User.Id == Session.Current.User.Id
                );

                CanManageChannels =
                    currentMember?.Role == MemberRole.Owner ||
                    currentMember?.Role == MemberRole.Admin;
            }
            catch
            {
                CanManageChannels = false;
            }
        }

        private void CreateChannel()
        {
            if (!CanManageChannels)
                return;

            var window = new CreateChannelView
            {
                Owner = Application.Current.MainWindow
            };

            bool? result = window.ShowDialog();

            if (result != true)
                return;

            Channels.Add(new ChannelViewModel((id) => OnChannelSelected?.Invoke(id))
            {
                Id = Guid.NewGuid().ToString(),
                Name = window.ChannelName,
                ServerID = _currentServerId,
                CreatedAt = DateTime.Now
            });
        }

        private void DeleteChannel(ChannelViewModel? channel)
        {
            if (!CanManageChannels)
                return;

            if (channel == null)
                return;

            var confirm = MessageBox.Show(
                $"Voulez-vous vraiment supprimer le canal #{channel.Name} ?",
                "Supprimer le canal",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirm != MessageBoxResult.Yes)
                return;

            Channels.Remove(channel);
        }
    }
}