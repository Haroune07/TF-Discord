using Frontend.Commands;
using Frontend.ViewModels.Base;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;


namespace Frontend.ViewModels
{
    public class ServerViewModel : ObservableObject
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string? ServerImageUrl { get; set; }
        public string Initials => Name.Length >= 2 ? Name.Substring(0, 2).ToUpper() : Name.ToUpper();
        public ICommand SelectCommand { get; }

        public ServerViewModel(string name, string id, Action<string> onSelected, string? serverImageUrl = null)
        {
            Name = name;
            Id = id;
            ServerImageUrl = serverImageUrl;
            SelectCommand = new RelayCommand(() => onSelected(Id), () => true);
        }
    }
}