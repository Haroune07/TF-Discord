using Frontend.Commands;
using Frontend.Global;
using Frontend.ViewModels.Base;
using Shared.DTOs;
using System;
using System.Windows.Input;

namespace Frontend.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private MainViewModel? _main;

        public UserDTO? User { get; private set; } = Session.Current.User;

        public bool IsUserOnline => User?.IsOnline == true;
        public string OnlineStatus => IsUserOnline ? "Online" : "Offline";

        public string MemberSince =>
            User != null
                ? $"Member since {User.CreatedAt:MMMM dd, yyyy}"
                : "Member since unknown";

        public ICommand? GoToLoginCommand { get; }

        public HomeViewModel(MainViewModel main)
        {
            _main = main;
            GoToLoginCommand = new RelayCommand(Logout, () => true);
        }

        private void Logout()
        {
            Session.Current.Logout();
            _main!.CurrentViewModel = new LoginViewModel(_main);
        }
    }
}