using Frontend.Global;
using Frontend.Services;
using Frontend.Views;
using Shared.DTOs;
using Shared.DTOs.Requests;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public partial class ChannelListViewModel : ObservableObject
    {
        public ObservableCollection<ChannelViewModel> Channels { get; set; }

        private readonly IApiService _apiService;

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

        public ChannelListViewModel(IApiService apiService)
        {
            _apiService = apiService;

            Channels = new ObservableCollection<ChannelViewModel>();

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

            if (Session.Current.User == null)
                return;

            var data = await _apiService.GetServerChannelsAsync(serverId, Session.Current.User.Id);

            foreach (var c in data.Where(c => c.ServerId == serverId))
            {
                Channels.Add(new ChannelViewModel((id) => HandleSelect(id))
                {
                    Name = c.Name ?? string.Empty,
                    Id = c.Id,
                    ServerID = c.ServerId ?? string.Empty,
                    CreatedAt = c.CreatedAt
                });
            }
        }

        private void HandleSelect(string channelId)
        {
            OnChannelSelected?.Invoke(channelId);
        }

        private async Task LoadCurrentUserRoleAsync(string serverId)
        {
            CanManageChannels = false;

            if (Session.Current.User == null)
                return;

            try
            {
                var members = await _apiService.GetServerMembersAsync(serverId);
                var currentMember = members.FirstOrDefault(m => m.User.Id == Session.Current.User.Id);

                CanManageChannels =
                    currentMember?.Role == MemberRole.Owner ||
                    currentMember?.Role == MemberRole.Admin;
            }
            catch
            {
                CanManageChannels = false;
            }
        }

        private async void CreateChannel()
        {
            if (!CanManageChannels || Session.Current.User == null)
                return;

            var window = new CreateChannelView
            {
                Owner = Application.Current.MainWindow
            };

            bool? result = window.ShowDialog();

            if (result != true)
                return;

            var created = await _apiService.CreateServerChannelAsync(
                Session.Current.User.Id,
                new CreateChannelRequest
                {
                    Name = window.ChannelName,
                    ServerId = _currentServerId
                });

            if (created == null)
            {
                MessageBox.Show(
                    "Impossible de créer le canal. Vérifiez vos permissions.",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            Channels.Add(new ChannelViewModel((id) => OnChannelSelected?.Invoke(id))
            {
                Id = created.Id,
                Name = created.Name ?? window.ChannelName,
                ServerID = created.ServerId ?? _currentServerId,
                CreatedAt = created.CreatedAt
            });
        }

        private async void DeleteChannel(ChannelViewModel? channel)
        {
            if (!CanManageChannels || Session.Current.User == null)
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

            var deleted = await _apiService.DeleteChannelAsync(channel.Id, Session.Current.User.Id);
            if (!deleted)
            {
                MessageBox.Show(
                    "Impossible de supprimer le canal. Vérifiez vos permissions.",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            Channels.Remove(channel);
        }
    }
}