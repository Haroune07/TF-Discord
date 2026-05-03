using Frontend.Commands;
using Frontend.Global;
using Frontend.Services;
using Frontend.ViewModels.Base;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        private readonly ApiService _apiService = new();

        private MainViewModel? _main;

        public IRelayCommand? GoToLoginCommand { get; }

        public IAsyncRelayCommand? RegisterCommand { get; }

        
        public RegisterViewModel(MainViewModel main)
        {
            _main = main;
            GoToLoginCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => _main.CurrentViewModel = new LoginViewModel(_main), () => true);
            RegisterCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(Register, () => true);
        }

        public async Task Register()
        {
            ErrorMessage = string.Empty;
            var res = await _apiService.RegisterUserAsync(new() { Password = Password, Username = Username });

            if (res.Success && res.User != null)
            {
                Session.Current.Login(res.User);

                _main!.CurrentViewModel = new HomeViewModel(_main);
            }
            // Yassine
            else
            {
                ErrorMessage = res.Message;
            }
            // End

            Debug.WriteLine(JsonSerializer.Serialize(res));

        }

    }
}