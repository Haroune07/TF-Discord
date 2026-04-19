using System.ComponentModel;
using System.Runtime.CompilerServices;

public class UserViewModel : INotifyPropertyChanged
{
    private bool _isOnline;
    private string _profileImageUrl = string.Empty;


    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;

    public bool IsOnline
    {
        get => _isOnline;
        set { _isOnline = value; OnPropertyChanged(); }
    }

    public string ProfileImageUrl
    {
        get => _profileImageUrl;
        set { _profileImageUrl = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}