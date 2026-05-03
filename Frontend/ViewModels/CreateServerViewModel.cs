using Frontend.Global;
using Frontend.Services;
using Shared.DTOs.Requests;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public partial class CreateServerViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new();

        [ObservableProperty]
        private string serverName = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private string serverImageUrl = string.Empty;

        public IAsyncRelayCommand CreateCommand { get; }

        // Callback invoqué après création réussie
        public Action? OnCreated { get; set; }

        public CreateServerViewModel()
        {
            CreateCommand = new AsyncRelayCommand(Create, () => true);
        }

        private async Task Create()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(ServerName))
            {
                ErrorMessage = "Le nom du serveur ne peut pas être vide.";
                return;
            }

            var result = await _apiService.CreateServerAsync(new CreateServerRequest
            {
                Name = ServerName,
                OwnerId = Session.Current.User!.Id,
                ServerImageUrl = string.IsNullOrWhiteSpace(ServerImageUrl) ? null : ServerImageUrl
            });

            if (result != null)
                OnCreated?.Invoke();
            else
                ErrorMessage = "Erreur lors de la création du serveur.";
        }
    }
}