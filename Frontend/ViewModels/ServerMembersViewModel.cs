using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Services;
using Shared.DTOs;
using System.Collections.ObjectModel;

namespace Frontend.ViewModels
{
    public partial class ServerMembersViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        public ObservableCollection<ServerMemberDTO> Members { get; set; }

        public string CurrentServerId { get; set; } = string.Empty;

        public IRelayCommand<ServerMemberDTO> KickCommand { get; }

        public ServerMembersViewModel(ApiService apiService)
        {
            _apiService = apiService;

            Members = new ObservableCollection<ServerMemberDTO>();

            KickCommand = new RelayCommand<ServerMemberDTO>(async (member) =>
            {
                if (member == null)
                    return;

                //await _apiService.KickMemberAsync(CurrentServerId, member.User.Id);

                await LoadMembersAsync(CurrentServerId);
            });
        }

        public async Task LoadMembersAsync(string serverId)
        {
            CurrentServerId = serverId;

            Members.Clear();

            var members = new List<ServerMemberDTO>
            {
                new ServerMemberDTO
                {
                    User = new UserDTO
                    {
                        Id = "1",
                        Username = "Salah",
                        IsOnline = true
                    }
                },

                new ServerMemberDTO
                {
                    User = new UserDTO
                        {
                        Id = "2", Username = "Haroune",
                        IsOnline = false
                    }
                },

                new ServerMemberDTO
                {
                    User = new UserDTO
                    {
                        Id = "3",
                        Username = "Wilson",
                        IsOnline = true
                    }
                }
            };

            foreach (var member in members)
            {
                Members.Add(member);
            }

            await Task.CompletedTask;
        }
    }
}