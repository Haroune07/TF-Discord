using Frontend.Services;
using Frontend.ViewModels.Base;
using Shared.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModels
{
    public class SearchUserViewModel : BaseViewModel
    {
        public ObservableCollection<UserInviteViewModel> Users { get; set; }
        private readonly ApiService _apiService = new();

        public event Action<string>? OnUserSelect;
        public bool HasResults => Users.Count > 0;

        private string _inputText = string.Empty;
        public string InputText
        {
            get => _inputText;
            set
            {
                if (_inputText != value)
                {
                    _inputText = value;
                    OnPropertyChanged();
                    _ = LoadUserInvite(_inputText);
                }
            }
        }

        public SearchUserViewModel()
        {
            Users = new ObservableCollection<UserInviteViewModel>();
        }
        public string SelectedServerId { get; set; }

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
                        onInvite: async (id) => await HandleServerInvite(id)
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
            var channel = await _apiService.CreateDMAsync(new CreateDMRequest { TargetUserId = userId });
        }

        private async Task HandleServerInvite(string userId)
        {
            Console.WriteLine($"Invitation envoyée à l'ID : {userId}");
            
        }
    }
}
