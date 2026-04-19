using Frontend.Commands;
using Frontend.Global;
using Frontend.Services;
using Frontend.ViewModels.Base;
using Shared.DTOs.Requests;
using System.Windows.Input;

namespace Frontend.ViewModels
{
    public class CreateServerViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        private string _serverName = string.Empty;
        public string ServerName
        {
            get => _serverName;
            set { _serverName = value; OnPropertyChanged(); }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private string _serverImageUrl = string.Empty;
        public string ServerImageUrl
        {
            get => _serverImageUrl;
            set { _serverImageUrl = value; OnPropertyChanged(); }
        }
        public ICommand CreateCommand { get; }

        // Callback invoqué après création réussie
        public Action? OnCreated { get; set; }

        public CreateServerViewModel()
        {
            CreateCommand = new RelayCommand(Create, () => true);
        }

        private async void Create()
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