using Frontend.Commands;
using Frontend.Services;
using Frontend.ViewModels.Base;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Input;

namespace Frontend.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        private string _username = "";
        private string _password = "";
        private readonly ApiService _apiService = new();

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login, () => true);
        }

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

        public async void Login()
        {
            var res = await _apiService.LoginUserAsync(new() { Password = _password, Username = _username });

            Debug.WriteLine("\n\n\n");
            Debug.WriteLine(JsonSerializer.Serialize(res));
        }
    }
}
