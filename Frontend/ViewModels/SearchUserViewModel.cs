using Frontend.Services;
using Shared.DTOs.Requests;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public partial class SearchUserViewModel : ObservableObject
    {
        public ObservableCollection<UserInviteViewModel> Users { get; set; } = new();
        private readonly ApiService _apiService = new();

        public event Action<string>? OnDMRequest;

        public bool HasResults => Users.Count > 0;

        [ObservableProperty]
        private string inputText = string.Empty;

        partial void OnInputTextChanged(string value)
        {
            _ = LoadUserInvite(value);
        }

        public string SelectedServerId { get; set; } = string.Empty;

        public SearchUserViewModel()
        {
        }

        public async Task LoadUserInvite(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                Users.Clear();
                OnPropertyChanged(nameof(HasResults));
                return;
            }

            try
            {
                var data = await _apiService.SearchUsersAsync(query);
                Users.Clear();

                foreach (var c in data)
                {
                    Users.Add(new UserInviteViewModel(
                        onDM: async (id) => await HandleOpenDM(id),
                        onInvite: async (id) => await HandleServerInvite(id, c.Username)
                    )
                    {
                        Id = c.Id,
                        Username = c.Username,
                        ProfileImageUrl = c.ProfileImageUrl
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur LoadUserInvite: {ex.Message}");
            }
            finally
            {
                OnPropertyChanged(nameof(HasResults));
            }
        }

        private async Task HandleOpenDM(string userId)
        {
            if (Global.Session.Current.User == null) return;

            var channel = await _apiService.CreateDMAsync(new CreateDMRequest 
            { 
                SenderId = Global.Session.Current.User.Id,
                TargetUserId = userId 
            });

            if (channel != null)
            {
                // Clear search to provide feedback and close popup
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    InputText = string.Empty;
                    OnDMRequest?.Invoke(channel.Id);
                });
            }
        }

        private async Task HandleServerInvite(string userId, string username)
        {
            if (Global.Session.Current.User == null) return;

            var success = await _apiService.SendFriendRequestAsync(Global.Session.Current.User.Id, username);
            if (success)
            {
                InputText = string.Empty;
                System.Windows.MessageBox.Show($"Friend request sent to {username}!", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("Could not send friend request. You might already be friends or a request is pending.", "Info", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }
    }
}
