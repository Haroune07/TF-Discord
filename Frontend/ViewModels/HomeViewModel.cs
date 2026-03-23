using Frontend.Commands;
using Frontend.Global;
using Frontend.ViewModels.Base;
using Shared.DTOs;
using System.Diagnostics;
using System.Windows.Input;
namespace Frontend.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private MainViewModel? _main;

        public UserDTO? User { private set; get; } = Session.Current.User;
        public string OnlineStatus => User.IsOnline ? "Online" : "Offline";

        public ICommand? GoToLoginCommand { get;}

        public HomeViewModel(MainViewModel main)
        {
            _main = main;
            GoToLoginCommand = new RelayCommand(() => main.CurrentViewModel = new LoginViewModel(_main), () => true);
        }
    }
}
