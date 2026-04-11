using Frontend.Commands;
using Frontend.Services;
using Frontend.ViewModels.Base;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Input;
using Frontend.Global;

namespace Frontend.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username = "";
        private string _password = "";
        private string _errormessage = "";

        private readonly ApiService _apiService = new();

        private MainViewModel? _main;
        public ICommand? GoToRegisterCommand { get; }

        public ICommand? LoginCommand { get; }

        public LoginViewModel(MainViewModel main)
        {
            _main = main;
            LoginCommand = new RelayCommand(Login, () => true);
            GoToRegisterCommand = new RelayCommand(() => main.CurrentViewModel = new RegisterViewModel(_main), () => true);
        }

        public LoginViewModel() { }

        public string UserName
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            private get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errormessage;
            set
            {
                _errormessage = value;
                OnPropertyChanged();
            }
        }

        public async void Login()
        {
            ErrorMessage = "";

            var res = await _apiService.LoginUserAsync(new() { Password = _password, Username = _username });

            if (res.Success && res.User != null)
            {
                Session.Current.Login(res.User);

                _main.CurrentViewModel = new HomeViewModel(_main);
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