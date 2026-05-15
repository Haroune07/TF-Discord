using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Global;
using Frontend.Services;
using Shared.Enums;
using System.Collections.ObjectModel;
using System.Windows;

namespace Frontend.ViewModels
{
    public partial class ServerMembersViewModel : ObservableObject
    {
        private readonly IApiService _apiService;
        private int _loadVersion;

        public ObservableCollection<ServerMemberItemViewModel> Members { get; } = new();

        [ObservableProperty]
        private bool canKickMembers;

        public string CurrentServerId { get; private set; } = string.Empty;

        public IRelayCommand<ServerMemberItemViewModel> KickCommand { get; }

        public ServerMembersViewModel(IApiService apiService)
        {
            _apiService = apiService;
            KickCommand = new RelayCommand<ServerMemberItemViewModel>(KickMember, CanExecuteKick);
        }

        public async Task LoadMembersAsync(string serverId)
        {
            var version = Interlocked.Increment(ref _loadVersion);
            CurrentServerId = serverId;
            Members.Clear();
            CanKickMembers = false;

            if (string.IsNullOrEmpty(serverId) || Session.Current.User == null)
                return;

            try
            {
                var members = await _apiService.GetServerMembersAsync(serverId);

                if (version != _loadVersion)
                    return;

                var currentMember = members.FirstOrDefault(m => m.User.Id == Session.Current.User.Id);
                CanKickMembers = currentMember?.Role is MemberRole.Owner or MemberRole.Admin;

                var uniqueMembers = members
                    .GroupBy(m => m.User.Id, StringComparer.Ordinal)
                    .Select(g => g.OrderByDescending(m => m.Role).First())
                    .OrderByDescending(m => m.User.IsOnline)
                    .ThenBy(m => m.User.Username, StringComparer.OrdinalIgnoreCase);

                Members.Clear();
                foreach (var member in uniqueMembers)
                    Members.Add(new ServerMemberItemViewModel(member, CanKickMembers));
            }
            catch
            {
                if (version == _loadVersion)
                    CanKickMembers = false;
            }
        }

        public void Clear()
        {
            Interlocked.Increment(ref _loadVersion);
            CurrentServerId = string.Empty;
            Members.Clear();
            CanKickMembers = false;
        }

        private static bool CanExecuteKick(ServerMemberItemViewModel? item) => item?.CanKick == true;

        private async void KickMember(ServerMemberItemViewModel? item)
        {
            if (item == null || Session.Current.User == null || string.IsNullOrEmpty(CurrentServerId))
                return;

            var confirm = MessageBox.Show(
                $"Expulser {item.Username} du serveur ?",
                "Expulser un membre",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            var kicked = await _apiService.KickMemberAsync(
                CurrentServerId,
                Session.Current.User.Id,
                item.Member.User.Id);

            if (!kicked)
            {
                MessageBox.Show(
                    "Impossible d'expulser ce membre. Vérifiez vos permissions.",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var toRemove = Members.Where(m => m.Member.User.Id == item.Member.User.Id).ToList();
            foreach (var member in toRemove)
                Members.Remove(member);
        }
    }
}
