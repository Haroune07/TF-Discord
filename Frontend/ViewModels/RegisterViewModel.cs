using Frontend.Commands;
using Frontend.Global;
using Frontend.Services;
using Frontend.ViewModels.Base;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Input;

namespace Frontend.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string _username = "";
        private string _password = "";
        private readonly ApiService _apiService = new();

        private MainViewModel? _main;

        public ICommand? GoToLoginCommand { get; }

        public ICommand? RegisterCommand { get; }

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
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }
        public RegisterViewModel(MainViewModel main)
        {
            _main = main;
            GoToLoginCommand = new RelayCommand(() => _main.CurrentViewModel = new LoginViewModel(_main), () => true);
            RegisterCommand = new RelayCommand(Register, () => true);
        }

        public async void Register()
        {
            var res = await _apiService.RegisterUserAsync(new() { Password = _password, Username = _username});

            if (res.Success && res.User != null)
            {
                Session.Current.Login(res.User);
            }

            Debug.WriteLine(JsonSerializer.Serialize(res));

        }

    }
}