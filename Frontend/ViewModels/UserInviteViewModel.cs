using Frontend.Commands;
using Frontend.ViewModels.Base;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

public class UserInviteViewModel : BaseViewModel, INotifyPropertyChanged
{
    private string _username;
    public event PropertyChangedEventHandler PropertyChanged;
    public string Id { get; set; } = string.Empty;
    public string Username { get => _username; set { if (_username != value) { _username = value; OnPropertyChanged(); } } }
    public bool IsOnline { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ProfileImageUrl { get; set; }
    public ICommand DMCommand { get; }
    public ICommand InviteServerCommand { get; }

    public UserInviteViewModel(Action<string> onDM, Action<string> onInvite)
    {
        _username = string.Empty;
        DMCommand = new RelayCommand(() => onDM(Id), () => !string.IsNullOrEmpty(Id));
        InviteServerCommand = new RelayCommand(() => onInvite(Id), () => !string.IsNullOrEmpty(Id));
    }

    protected void OnPropertyChanged([CallerMemberName] string username = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(username));
    }
}

