using Frontend.ViewModels.Base;

namespace Frontend.ViewModels
{
    internal class RegisterViewModel : BaseViewModel
    {
        private string _username = "";
        private string _password = "";

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
    }
}