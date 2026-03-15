using Frontend.Global;
using Frontend.ViewModels.Base;
using Shared.DTOs;
namespace Frontend.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private MainViewModel _main;

        public UserDTO? User { private set; get; } = Session.Current.User;
        public string OnlineStatus => User.IsOnline ? "Online" : "Offline";

        public HomeViewModel(MainViewModel main)
        {
            _main = main;
        }
    }
}
