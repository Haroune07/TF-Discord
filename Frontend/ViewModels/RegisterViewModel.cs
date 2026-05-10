using Frontend.Global;
using Frontend.Services;
using Shared.Constants;
using System.Diagnostics;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Frontend.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {

        [ObservableProperty]
        private string userName = string.Empty;

        [ObservableProperty]
        private string phoneNumber = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string confirmPassword = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        private readonly ApiService _apiService;
        private readonly IServiceProvider _services;

        private MainViewModel? _main;

        public IRelayCommand? GoToLoginCommand { get; }

        public IAsyncRelayCommand? RegisterCommand { get; }

        
        public RegisterViewModel(MainViewModel main, ApiService apiService, IServiceProvider services)
        {
            _main = main;
            _apiService = apiService;
            _services = services;
            GoToLoginCommand = new RelayCommand(() => { main.CurrentViewModel = ActivatorUtilities.CreateInstance<LoginViewModel>(_services, _main!); });
            RegisterCommand = new AsyncRelayCommand(Register, () => true);
        }

        public async Task Register()
        {
            ErrorMessage = string.Empty;
            if (!string.Equals(Password, ConfirmPassword, StringComparison.Ordinal))
            {
                ErrorMessage = Messages.PasswordMismatch;
                return;
            }
            var res = await _apiService.RegisterUserAsync(new() { Password = Password, Username = UserName, PhoneNumber = PhoneNumber });

            if (res.Success && res.User != null)
            {
                Session.Current.Login(res.User);

                _main!.CurrentViewModel = ActivatorUtilities.CreateInstance<HomeViewModel>(_services, _main);
            }
            else
            {
                ErrorMessage = res.Message;
            }

            Debug.WriteLine(JsonSerializer.Serialize(res));

        }

    }
}
