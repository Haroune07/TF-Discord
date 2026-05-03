using Frontend.Global;
using Frontend.Services;
using System.Diagnostics;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Frontend.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {

        [ObservableProperty]
        private string userName = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        private readonly ApiService _apiService = new();

        private MainViewModel? _main;
        public IRelayCommand? GoToRegisterCommand { get; }

        public IAsyncRelayCommand? LoginCommand { get; }

        public LoginViewModel(MainViewModel main)
        {
            _main = main;
            LoginCommand = new AsyncRelayCommand(Login, () => true);
            GoToRegisterCommand = new RelayCommand(() => { if (_main != null) _main.CurrentViewModel = new RegisterViewModel(_main); });
        }

        public LoginViewModel() { }




        public async Task Login()
        {
            ErrorMessage = string.Empty;

            var res = await _apiService.LoginUserAsync(new() { Password = Password, Username = UserName });

            if (res.Success && res.User != null)
            {
                Session.Current.Login(res.User);

                _main?.CurrentViewModel = new HomeViewModel(_main);
            }

            else
            {
                ErrorMessage = res.Message;
            }

            Debug.WriteLine("\n\n\n");
            Debug.WriteLine(JsonSerializer.Serialize(res));

        }
    }
}