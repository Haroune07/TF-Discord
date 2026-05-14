using Frontend.Global;
using Frontend.Services;
using Shared.DTOs;
using Shared.DTOs.Requests;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public partial class UserListViewModel : ObservableObject
    {
        private readonly IApiService _apiService;
        private readonly IDispatcherService _dispatcher;
        private readonly Action<string> _onDMChannelReady;

        public IRelayCommand OpenDMCommand { get; }
        public IRelayCommand AcceptRequestCommand { get; }
        public IRelayCommand DeclineRequestCommand { get; }

        public ObservableCollection<UserDTO> Users { get; set; } = new();
        public ObservableCollection<FriendshipDTO> PendingRequests { get; set; } = new();

        public UserListViewModel(IApiService apiService, IDispatcherService dispatcher, Action<string> onDMChannelReady)
        {
            _apiService = apiService;
            _dispatcher = dispatcher;
            _onDMChannelReady = onDMChannelReady;
            OpenDMCommand = new AsyncRelayCommand<UserDTO>(async (user) => await OpenDMAsync(user!), (user) => user != null);
            AcceptRequestCommand = new AsyncRelayCommand<FriendshipDTO>(async (req) => await UpdateRequestStatus(req!, Shared.Enums.FriendshipStatus.Accepted), (req) => req != null);
            DeclineRequestCommand = new AsyncRelayCommand<FriendshipDTO>(async (req) => await UpdateRequestStatus(req!, Shared.Enums.FriendshipStatus.Declined), (req) => req != null);
        }

        public async Task LoadUsersAsync()
        {
            if (Session.Current.User == null) return;

            // Load data from API first
            var dmChannels = await _apiService.GetMyDMChannelsAsync(Session.Current.User.Id);
            var friendships = await _apiService.GetFriendsAsync(Session.Current.User.Id);
            var pending = await _apiService.GetPendingRequestsAsync(Session.Current.User.Id);

            System.Diagnostics.Debug.WriteLine($"UserList: Found {dmChannels.Count} DM channels, {friendships.Count} friends, {pending.Count} pending.");

            // Update UI on the main thread to prevent crashes
            _dispatcher.Invoke(() =>
            {
                Users.Clear();
                
                // 1. Add people from DM history
                foreach (var channel in dmChannels)
                {
                    var otherUser = channel.Participants?.FirstOrDefault(p => p.Id != Session.Current.User.Id);
                    if (otherUser != null && !Users.Any(u => u.Id == otherUser.Id))
                    {
                        Users.Add(otherUser);
                    }
                }

                // 2. Add confirmed friends who might not have a DM channel yet
                foreach (var f in friendships)
                {
                    if (f.Friend != null && !Users.Any(u => u.Id == f.Friend.Id))
                    {
                        Users.Add(f.Friend);
                    }
                }

                // 3. Update Pending Requests
                PendingRequests.Clear();
                foreach (var p in pending)
                    PendingRequests.Add(p);
            });
        }

        public async Task OpenDMAsync(UserDTO targetUser)
        {
            if (Session.Current.User == null) return;

            var channel = await _apiService.CreateDMAsync(new CreateDMRequest
            {
                SenderId = Session.Current.User.Id,
                TargetUserId = targetUser.Id
            });

            if (channel != null)
                _onDMChannelReady(channel.Id);
        }

        private async Task UpdateRequestStatus(FriendshipDTO req, Shared.Enums.FriendshipStatus status)
        {
            var success = await _apiService.UpdateFriendshipStatusAsync(req.Id, Session.Current.User!.Id, status);
            if (success)
            {
                await LoadUsersAsync();
            }
        }
    }
}