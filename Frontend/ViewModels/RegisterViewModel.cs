using Frontend.Commands;
using Frontend.Services;
using Frontend.ViewModels.Base;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Input;

namespace Frontend.ViewModels
{
    internal class RegisterViewModel : BaseViewModel
    {
        private string _username = "";
        private string _password = "";
        private readonly ApiService _apiService = new();

        public ICommand RegisterCommand { get; }

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

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(Register, () => true);
        }

        public async void Register()
        {
            var res = await _apiService.RegisterUserAsync(new() { Password = _password, Username = _username});
            Debug.WriteLine(JsonSerializer.Serialize(res));

        }

    }
}